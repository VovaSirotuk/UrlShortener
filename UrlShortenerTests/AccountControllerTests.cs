using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerTestProject.Controllers;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Services.AboutService;
using UrlShortenerTestProject.ViewModels;

namespace UrlShortenerTests
{
    public class AccountControllerTests
    {
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockUserManager = MockUserManager<User>();
            _mockSignInManager = MockSignInManager<User>();

            _controller = new AccountController(_mockSignInManager.Object, _mockUserManager.Object);
        }

        [Fact]
        public void Login_ReturnsView()
        {
            // Act
            var result = _controller.Login() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_RedirectsToHome()
        {
            // Arrange
            var model = new LoginVM { Email = "test@example.com", Password = "Password123!", RememberMe = false };
            _mockSignInManager.Setup(s => s.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginVM { Email = "test@example.com", Password = "WrongPassword", RememberMe = false };
            _mockSignInManager.Setup(s => s.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _controller.Login(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public void Register_ReturnsView()
        {
            // Act
            var result = _controller.Register() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Register_ValidModel_RedirectsToLogin()
        {
            // Arrange
            var model = new RegisterVM { Name = "Test User", Email = "test@example.com", Password = "Password123!" };
            var user = new User { FullName = model.Name, Email = model.Email, UserName = model.Email };

            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), model.Password))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewWithErrors()
        {
            // Arrange
            var model = new RegisterVM { Name = "Test User", Email = "invalid-email", Password = "short" };
            var identityErrors = new List<IdentityError> { new IdentityError { Description = "Invalid email" } };
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), model.Password))
                            .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            // Act
            var result = await _controller.Register(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Logout_RedirectsToHome()
        {
            // Act
            var result = await _controller.Logout() as RedirectToActionResult;

            // Assert
            _mockSignInManager.Verify(s => s.SignOutAsync(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            var userManager = MockUserManager<TUser>().Object;
            return new Mock<SignInManager<TUser>>(userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null, null, null, null);
        }
    }
}
