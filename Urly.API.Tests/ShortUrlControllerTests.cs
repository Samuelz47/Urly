using Microsoft.AspNetCore.Mvc;
using Moq;
using Urly.API.Controllers;
using Urly.Application.DTOs;
using Urly.Application.Interfaces;
using Xunit;

namespace Urly.API.Tests.Controllers
{
    public class ShortUrlControllerTests
    {
        private readonly Mock<IShortUrlService> _mockShortUrlService;

        private readonly ShortUrlController _controller;

        public ShortUrlControllerTests()
        {
            _mockShortUrlService = new Mock<IShortUrlService>();

            _controller = new ShortUrlController(_mockShortUrlService.Object);
        }

        #region CreateShortUrl (POST /) Tests

        [Fact]
        public async Task CreateShortUrl_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var registrationDto = new ShortUrlForRegistrationDTO { LongURL = "https://google.com" };
            var expectedDto = new ShortUrlDTO
            {
                ShortCode = "ABCDEFG",
                FullShortUrl = "https://localhost:7032/ABCDEFG",
                LongURL = "https://google.com"
            };

            _mockShortUrlService.Setup(s => s.CreateShortUrlAsync(registrationDto))
                                .ReturnsAsync(expectedDto);

            // Act
            var result = await _controller.CreateShortUrl(registrationDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);

            Assert.Equal(nameof(ShortUrlController.GetAnalytics), createdResult.ActionName);

            Assert.Equal(expectedDto.ShortCode, createdResult.RouteValues["code"]);

            Assert.Equal(expectedDto, createdResult.Value);
        }

        #endregion

        #region Redirection (GET /{code}) Tests

        [Fact]
        public async Task Redirection_ShouldReturnRedirect_WhenCodeExists()
        {
            // Arrange
            var shortCode = "ABCDEFG";
            var expectedLongUrl = "https://google.com";

            _mockShortUrlService.Setup(s => s.GetLongUrlAndRegisterClickAsync(shortCode))
                                .ReturnsAsync(expectedLongUrl);

            // Act
            var result = await _controller.Redirection(shortCode);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal(expectedLongUrl, redirectResult.Url);
        }

        [Fact]
        public async Task Redirection_ShouldReturnNotFound_WhenCodeDoesNotExist()
        {
            // Arrange
            var shortCode = "CODIGO_FALSO";

            _mockShortUrlService.Setup(s => s.GetLongUrlAndRegisterClickAsync(shortCode))
                                .ReturnsAsync((string?)null);

            // Act
            var result = await _controller.Redirection(shortCode);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        #endregion

        #region GetAnalytics (GET /{code}/stats) Tests

        [Fact]
        public async Task GetAnalytics_ShouldReturnOk_WhenCodeExists()
        {
            // Arrange
            var shortCode = "ABCDEFG";
            var expectedAnalytics = new UrlAnalyticsDTO { TotalClicks = 123 };

            _mockShortUrlService.Setup(s => s.GetUrlAnalyticsAsync(shortCode))
                                .ReturnsAsync(expectedAnalytics);

            // Act
            var result = await _controller.GetAnalytics(shortCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(expectedAnalytics, okResult.Value);
        }

        [Fact]
        public async Task GetAnalytics_ShouldReturnNotFound_WhenCodeDoesNotExist()
        {
            // Arrange
            var shortCode = "CODIGO_FALSO";

            _mockShortUrlService.Setup(s => s.GetUrlAnalyticsAsync(shortCode))
                                .ReturnsAsync((UrlAnalyticsDTO?)null);

            // Act
            var result = await _controller.GetAnalytics(shortCode);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        #endregion
    }
}