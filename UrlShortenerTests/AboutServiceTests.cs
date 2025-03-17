using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.AboutRep;
using UrlShortenerTestProject.Services.AboutService;
namespace UrlShortenerTests
{
    public class AboutServiceTests
    {
        private readonly Mock<IAboutRepository> _mockRepository;
        private readonly AboutService _aboutService;

        public AboutServiceTests()
        {
            _mockRepository = new Mock<IAboutRepository>();
            _aboutService = new AboutService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetDescriptionAsync_ReturnsDescription_WhenAboutInfoExists()
        {
            // Arrange
            var aboutInfo = new AboutInfo { Description = "Test Description" };
            _mockRepository.Setup(repo => repo.GetAboutInfoAsync()).ReturnsAsync(aboutInfo);

            // Act
            var result = await _aboutService.GetDescriptionAsync();

            // Assert
            Assert.Equal("Test Description", result);
        }

        [Fact]
        public async Task GetDescriptionAsync_ReturnsDefaultMessage_WhenAboutInfoIsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAboutInfoAsync()).ReturnsAsync((AboutInfo?)null);

            // Act
            var result = await _aboutService.GetDescriptionAsync();

            // Assert
            Assert.Equal("No description available.", result);
        }

        [Fact]
        public async Task UpdateDescriptionAsync_CallsRepositoryMethod()
        {
            // Arrange
            var newDescription = "Updated Description";

            // Act
            await _aboutService.UpdateDescriptionAsync(newDescription);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAboutInfoAsync(newDescription), Times.Once);
        }
    }
}