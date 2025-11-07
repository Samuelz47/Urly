using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urly.Application.Common;
using Urly.Application.DTOs;
using Urly.Application.Interfaces;
using Urly.Domain.Entities;
using Urly.Domain.Repositories;

namespace Urly.Application.Services;
public class ShortUrlService : IShortUrlService
{
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly IUrlClickRepository _urlClickRepository;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;

    public ShortUrlService(IShortUrlRepository shortUrlRepository, IUnitOfWork uow, IUrlClickRepository urlClickRepository, IMapper mapper, IDistributedCache cache, IConfiguration configuration)
    {
        _shortUrlRepository = shortUrlRepository;
        _uow = uow;
        _urlClickRepository = urlClickRepository;
        _mapper = mapper;
        _cache = cache;
        _configuration = configuration;
    }

    public async Task<ShortUrlDTO> CreateShortUrlAsync(ShortUrlForRegistrationDTO createDto)
    {
        string shortCode;

        while (true)
        {
            shortCode = ShortCodeGenerator.GenerateRandomShortCode();

            bool codeIsUnique = await _shortUrlRepository.IsCodeUniqueAsync(shortCode);

            if (codeIsUnique) { break; }
        }

        var shortUrl = _mapper.Map<ShortUrl>(createDto);
        shortUrl.ShortCode = shortCode;
        shortUrl.CreateAtUtc = DateTime.UtcNow;

        _shortUrlRepository.Create(shortUrl);
        await _uow.CommitAsync();

        var responseDto = _mapper.Map<ShortUrlDTO>(shortUrl);
        var baseUrl = _configuration["AppBaseUrl"];
        responseDto.FullShortUrl = $"{baseUrl}/api/{responseDto.ShortCode}";
        return responseDto;
    }

    public async Task<string?> GetLongUrlAndRegisterClickAsync(string code)
    {
        string? longUrl;
        string cacheKey = $"url:{code}";

        longUrl = await _cache.GetStringAsync(cacheKey);

        ShortUrl? shortUrl = null;

        if (string.IsNullOrEmpty(longUrl))
        {
            shortUrl = await _shortUrlRepository.GetAsync(c => c.ShortCode == code);
            if (shortUrl is null) { return null; }

            longUrl = shortUrl.LongURL;

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await _cache.SetStringAsync(cacheKey, longUrl, cacheOptions);
        }

        if (shortUrl is null)
        {
            shortUrl = await _shortUrlRepository.GetAsync(c => c.ShortCode == code);
            if (shortUrl is null) return null;
        }

        var urlClick = new UrlClick
        {
            ShortUrlId = shortUrl.Id,
            ClickedAtUtc = DateTime.UtcNow
        };

        _urlClickRepository.Create(urlClick);
        await _uow.CommitAsync();

        return longUrl;
    }

    public async Task<UrlAnalyticsDTO?> GetUrlAnalyticsAsync(string code)
    {
        var shortUrl = await _shortUrlRepository.GetAsync(c => c.ShortCode == code);

        if (shortUrl is null) { return null; }

        var totalClicks = await _urlClickRepository.CountByShortUrlIdAsync(shortUrl.Id);

        var urlAnalyticsDto = new UrlAnalyticsDTO { TotalClicks = totalClicks };
        return urlAnalyticsDto;
    }
}
