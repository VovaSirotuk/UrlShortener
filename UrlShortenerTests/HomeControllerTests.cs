using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerTestProject.Controllers;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Services.UrlSer;
using UrlShortenerTestProject.ViewModels;

namespace UrlShortenerTests
{
    public class HomeControllerTests
    {
        private readonly Mock<IUrlShortService> _mockUrlService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockUrlService = new Mock<IUrlShortService>();
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);
            _controller = new HomeController(_mockUrlService.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task Table_ReturnsViewWithUrls()
        {
            // Arrange
            var urls = new List<ShortenedUrl>
            {
                new ShortenedUrl { Id = 1, OriginalUrl = "https://example.com", ShortCode = "abc123" }
            };
            _mockUrlService.Setup(s => s.GetAll()).ReturnsAsync(urls);

            // Act
            var result = await _controller.Table();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ShortenedUrl>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task ShortenUrlFromTable_ValidInput_ReturnsOk()
        {
            // Arrange
            var model = new AddUrlVM { OriginalUrl = "https://example.com", CreatedBy = "user@user.com" };
            var shortenedUrl = new ShortenedUrl
            {
                Id = 1,
                OriginalUrl = model.OriginalUrl,
                ShortCode = "abc123",
                CreatedBy = "testUser",
                ClickCount = 0
            };

            // Налаштовуємо мокові значення
            _mockUrlService.Setup(s => s.GetByOriginCode(model.OriginalUrl)).ReturnsAsync((ShortenedUrl)null);
            _mockUrlService.Setup(s => s.ShortenUrlAsync(model)).ReturnsAsync(shortenedUrl);

            // Додаємо мок аутентифікації
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testUser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Додаємо мок `Request`
            _controller.ControllerContext.HttpContext.Request.Scheme = "https";
            _controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost");

            // Act
            var result = await _controller.ShortenUrlFromTable(model) as OkObjectResult;

            // Assert
            var response = result.Value;
            Assert.NotNull(response);

            // Використання рефлексії для перевірки властивостей
            Assert.Equal(shortenedUrl.Id, (int)response.GetType().GetProperty("id").GetValue(response, null));
            Assert.Equal(shortenedUrl.OriginalUrl, (string)response.GetType().GetProperty("originalUrl").GetValue(response, null));
            Assert.Equal(0, (int)response.GetType().GetProperty("clickCount").GetValue(response, null));
        }

        [Fact]
        public async Task RedirectToLongUrl_ValidShortCode_ReturnsRedirect()
        {
            // Arrange
            string shortCode = "abc123";
            string longUrl = "https://example.com";

            _mockUrlService.Setup(s => s.GetOriginalUrlAsync(shortCode)).ReturnsAsync(longUrl);

            // Act
            var result = await _controller.RedirectToLongUrl(shortCode);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(longUrl, redirectResult.Url);
        }

        [Fact]
        public async Task Delete_ExistingId_ReturnsSuccessJson()
        {
            // Arrange
            int id = 1;
            _mockUrlService.Setup(s => s.DeleteByIdAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var json = result.Value; // Не змінюємо тип
            Assert.True((bool)json.GetType().GetProperty("success")?.GetValue(json, null));
        }
    }
}

