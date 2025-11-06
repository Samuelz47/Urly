using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Linq.Expressions;
using System.Text;
using Urly.Application.Common;
using Urly.Application.DTOs;
using Urly.Application.Interfaces;
using Urly.Application.Services;
using Urly.Domain.Entities;
using Urly.Domain.Repositories;
using Xunit;

namespace Urly.Application.Tests.Services
{
    public class ShortUrlServiceTests
    {
        private readonly Mock<IShortUrlRepository> _mockShortUrlRepo;
        private readonly Mock<IUrlClickRepository> _mockUrlClickRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IDistributedCache> _mockCache;

        private readonly ShortUrlService _service;

        public ShortUrlServiceTests()
        {
            _mockShortUrlRepo = new Mock<IShortUrlRepository>();
            _mockUrlClickRepo = new Mock<IUrlClickRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockCache = new Mock<IDistributedCache>();

            _service = new ShortUrlService(
                _mockShortUrlRepo.Object,
                _mockUow.Object,
                _mockUrlClickRepo.Object,
                _mockMapper.Object,
                _mockCache.Object
            );
        }

        [Fact]
        public async Task CreateShortUrlAsync_ShouldCreateLink_WhenCodeIsUnique()
        {
            // Arrange
            var dto = new ShortUrlForRegistrationDTO { LongURL = "https://google.com" };
            var expectedEntity = new ShortUrl { Id = 1, LongURL = dto.LongURL, ShortCode = "ABCDEFG" };
            var expectedDto = new ShortUrlDTO { FullShortUrl = "https://urly.com/ABCDEFG", ShortCode = "ABCDEFG" };

            _mockShortUrlRepo.Setup(repo => repo.IsCodeUniqueAsync(It.IsAny<string>()))
                             .ReturnsAsync(true);

            _mockMapper.Setup(m => m.Map<ShortUrl>(dto)).Returns(expectedEntity);
            _mockMapper.Setup(m => m.Map<ShortUrlDTO>(expectedEntity)).Returns(expectedDto);

            // Act
            var result = await _service.CreateShortUrlAsync(dto);

            // Assert
            Assert.NotNull(result);
            _mockShortUrlRepo.Verify(repo => repo.IsCodeUniqueAsync(It.IsAny<string>()), Times.Once);
            _mockUow.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetLongUrlAndRegisterClickAsync_ShouldUseCache_WhenCacheHit()
        {
            // Arrange
            var shortCode = "ABCDEFG";
            var expectedLongUrl = "https://google.com-DO-CACHE";
            var cacheKey = $"url:{shortCode}";

            var expectedBytes = Encoding.UTF8.GetBytes(expectedLongUrl);

            _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedBytes);

            _mockShortUrlRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ShortUrl, bool>>>()))
                             .ReturnsAsync(new ShortUrl { Id = 1, LongURL = expectedLongUrl, ShortCode = shortCode });

            // Act
            var result = await _service.GetLongUrlAndRegisterClickAsync(shortCode);

            // Assert
            Assert.Equal(expectedLongUrl, result);

            _mockCache.Verify(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);

            _mockCache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);

            _mockUrlClickRepo.Verify(repo => repo.Create(It.IsAny<UrlClick>()), Times.Once);
            _mockUow.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetLongUrlAndRegisterClickAsync_ShouldUseDatabase_WhenCacheMiss()
        {
            // Arrange
            var shortCode = "ABCDEFG";
            var expectedLongUrl = "https://google.com-DO-BANCO";
            var cacheKey = $"url:{shortCode}";
            var shortUrlEntity = new ShortUrl { Id = 1, LongURL = expectedLongUrl, ShortCode = shortCode };

            _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[]?)null);

            _mockShortUrlRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ShortUrl, bool>>>()))
                             .ReturnsAsync(shortUrlEntity);

            // Act
            var result = await _service.GetLongUrlAndRegisterClickAsync(shortCode);

            // Assert
            Assert.Equal(expectedLongUrl, result);

            _mockCache.Verify(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);

            _mockCache.Verify(c => c.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);

            _mockUrlClickRepo.Verify(repo => repo.Create(It.IsAny<UrlClick>()), Times.Once);
            _mockUow.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUrlAnalyticsAsync_ShouldReturnTotalClicks_WhenCodeExists()
        {
            // Arrange
            var shortCode = "ABCDEFG";
            var shortUrlEntity = new ShortUrl { Id = 1, ShortCode = shortCode };
            var expectedClicks = 123;

            _mockShortUrlRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ShortUrl, bool>>>()))
                             .ReturnsAsync(shortUrlEntity);
            _mockUrlClickRepo.Setup(repo => repo.CountByShortUrlIdAsync(shortUrlEntity.Id))
                             .ReturnsAsync(expectedClicks);

            // Act
            var result = await _service.GetUrlAnalyticsAsync(shortCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedClicks, result.TotalClicks);
        }
    }
}