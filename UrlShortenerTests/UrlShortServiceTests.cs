using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.UrlRep;
using UrlShortenerTestProject.Services.UrlSer;
using UrlShortenerTestProject.ViewModels;

namespace UrlShortenerTests
{
    public class UrlShortServiceTests
    {
        private readonly Mock<IUrlRepository> _mockUrlRepository;
        private readonly UrlShortService _urlShortService;

        public UrlShortServiceTests()
        {
            _mockUrlRepository = new Mock<IUrlRepository>();
            _urlShortService = new UrlShortService(_mockUrlRepository.Object);
        }

        [Fact]
        public async Task ShortenUrlAsync_NewUrl_ReturnsShortenedUrl()
        {
            // Arrange
            var model = new AddUrlVM { OriginalUrl = "https://example.com", CreatedBy = "user1" };
            _mockUrlRepository.Setup(repo => repo.GetByOriginalUrlAsync(model.OriginalUrl)).ReturnsAsync((ShortenedUrl)null);
            _mockUrlRepository.Setup(repo => repo.GetByShortCodeAsync(It.IsAny<string>())).ReturnsAsync((ShortenedUrl)null);
            _mockUrlRepository.Setup(repo => repo.AddAsync(It.IsAny<ShortenedUrl>())).Returns(Task.CompletedTask);

            // Act
            var result = await _urlShortService.ShortenUrlAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.OriginalUrl, result.OriginalUrl);
            Assert.NotNull(result.ShortCode);
            Assert.Equal(model.CreatedBy, result.CreatedBy);
        }

        [Fact]
        public async Task GetByOriginCode_ExistingUrl_ReturnsShortenedUrl()
        {
            // Arrange
            var url = new ShortenedUrl { Id = 1, OriginalUrl = "https://example.com", ShortCode = "abc123" };
            _mockUrlRepository.Setup(repo => repo.GetByOriginalUrlAsync(url.OriginalUrl)).ReturnsAsync(url);

            // Act
            var result = await _urlShortService.GetByOriginCode(url.OriginalUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(url.OriginalUrl, result.OriginalUrl);
        }

        [Fact]
        public async Task GetOriginalUrlAsync_ValidShortCode_ReturnsOriginalUrl()
        {
            // Arrange
            var shortCode = "abc123";
            var url = new ShortenedUrl { Id = 1, OriginalUrl = "https://example.com", ShortCode = shortCode, ClickCount = 0 };
            _mockUrlRepository.Setup(repo => repo.GetByShortCodeAsync(shortCode)).ReturnsAsync(url);
            _mockUrlRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _urlShortService.GetOriginalUrlAsync(shortCode);

            // Assert
            Assert.Equal(url.OriginalUrl, result);
            Assert.Equal(1, url.ClickCount);
        }

        [Fact]
        public async Task GetOriginalUrlAsync_InvalidShortCode_ReturnsNull()
        {
            // Arrange
            _mockUrlRepository.Setup(repo => repo.GetByShortCodeAsync(It.IsAny<string>())).ReturnsAsync((ShortenedUrl)null);

            // Act
            var result = await _urlShortService.GetOriginalUrlAsync("invalid");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_ValidId_DeletesSuccessfully()
        {
            // Arrange
            int id = 1;
            _mockUrlRepository.Setup(repo => repo.DeleteByIdAsync(id)).Returns(Task.CompletedTask);

            // Act
            await _urlShortService.DeleteByIdAsync(id);

            // Assert
            _mockUrlRepository.Verify(repo => repo.DeleteByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfUrls()
        {
            // Arrange
            var urls = new List<ShortenedUrl> { new ShortenedUrl { Id = 1, OriginalUrl = "https://example.com", ShortCode = "abc123" } };
            _mockUrlRepository.Setup(repo => repo.GetAll()).ReturnsAsync(urls);

            // Act
            var result = await _urlShortService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}
