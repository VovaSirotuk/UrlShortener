using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerTestProject.Controllers;
using UrlShortenerTestProject.Services.AboutService;

namespace UrlShortenerTests
{
    public class AboutControllerTests
    {
        private readonly Mock<IAboutService> _mockAboutService;
        private readonly AboutController _controller;

        public AboutControllerTests()
        {
            _mockAboutService = new Mock<IAboutService>();
            _controller = new AboutController(_mockAboutService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithDescription()
        {
            // Arrange
            var expectedDescription = "This is a test description.";
            _mockAboutService.Setup(s => s.GetDescriptionAsync()).ReturnsAsync(expectedDescription);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(expectedDescription, result.Model);
        }

        [Fact]
        public async Task Update_ValidDescription_CallsUpdateAndRedirects()
        {
            // Arrange
            var newDescription = "New test description.";

            // Act
            var result = await _controller.Update(newDescription) as RedirectToActionResult;

            // Assert
            _mockAboutService.Verify(s => s.UpdateDescriptionAsync(newDescription), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task Update_EmptyDescription_DoesNotCallUpdateAndRedirects()
        {
            // Act
            var result = await _controller.Update("") as RedirectToActionResult;

            // Assert
            _mockAboutService.Verify(s => s.UpdateDescriptionAsync(It.IsAny<string>()), Times.Never);
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }
    }
}
