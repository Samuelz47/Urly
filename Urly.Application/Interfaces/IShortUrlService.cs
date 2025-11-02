using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urly.Application.DTOs;

namespace Urly.Application.Interfaces;
public interface IShortUrlService
{
    Task<ShortUrlDTO> CreateShortUrlAsync(ShortUrlForRegistrationDTO createDto);
    Task<string?> GetLongUrlAndRegisterClickAsync(string code);
    Task<UrlAnalyticsDTO?> GetUrlAnalyticsAsync(string code);
}
