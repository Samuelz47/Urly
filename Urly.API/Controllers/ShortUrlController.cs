using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Urly.Application.DTOs;
using Urly.Application.Interfaces;

namespace Urly.API.Controllers;
[Route("/")]
[ApiController]
[EnableRateLimiting("fixed-by-ip")]
public class ShortUrlController : ControllerBase
{
    private readonly IShortUrlService _shortUrlService;

    public ShortUrlController(IShortUrlService shortUrlService)
    {
        _shortUrlService = shortUrlService;
    }

    [HttpPost]
    public async Task<ActionResult<ShortUrlDTO>> CreateShortUrl(ShortUrlForRegistrationDTO dto)
    {
        var shortUrl = await _shortUrlService.CreateShortUrlAsync(dto);

        return CreatedAtAction(
                nameof(GetAnalytics), // Nome do método que busca o recurso
                new { code = shortUrl.ShortCode }, // Parâmetro da rota do outro método
                shortUrl // O objeto criado
            );
    }

    [HttpGet("{code}/stats", Name = "GetAnalytics")]
    public async Task<ActionResult<UrlAnalyticsDTO>> GetAnalytics(string code)
    {
        var urlAnalyticsDto = await _shortUrlService.GetUrlAnalyticsAsync(code);

        if (urlAnalyticsDto is null) { return NotFound("URL não encontrada"); }

        return Ok(urlAnalyticsDto);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Redirection(string code)
    {
        var longUrl = await _shortUrlService.GetLongUrlAndRegisterClickAsync(code);

        if (longUrl is null) { return NotFound("Link não encontrado ou expirado"); }

        return Redirect(longUrl);
    }
}
