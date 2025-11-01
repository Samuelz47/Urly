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
    private readonly IUnitOfWork _uow;

    public ShortUrlService(IShortUrlRepository shortUrlRepository, IUnitOfWork uow)
    {
        _shortUrlRepository = shortUrlRepository;
        _uow = uow;
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
}
