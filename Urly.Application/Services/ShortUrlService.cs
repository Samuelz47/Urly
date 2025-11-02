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

    public ShortUrlService(IShortUrlRepository shortUrlRepository, IUnitOfWork uow, IUrlClickRepository urlClickRepository)
    {
        _shortUrlRepository = shortUrlRepository;
        _uow = uow;
        _urlClickRepository = urlClickRepository;
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

        var shortUrl = new ShortUrl
        {
            LongURL = createDto.LongURL,
            ShortCode = shortCode,
            CreateAtUtc = DateTime.UtcNow
        };

        _shortUrlRepository.Create(shortUrl);
        await _uow.CommitAsync();

        var responseDto = new ShortUrlDTO { ShortURL = $"https://urly.com/{shortUrl.ShortCode}" };
        return responseDto;
    }

    public async Task<string?> GetLongUrlAndRegisterClickAsync(string code)
    {
        var shortUrl = await _shortUrlRepository.GetAsync(c => c.ShortCode == code);

        if (shortUrl is null) { return null; }

        var urlClick = new UrlClick
        {
            ShortUrlId = shortUrl.Id,
            ShortUrl = shortUrl,
            ClickedAtUtc = DateTime.UtcNow
        };

        _urlClickRepository.Create(urlClick);
        await _uow.CommitAsync();
        return shortUrl.LongURL;
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
