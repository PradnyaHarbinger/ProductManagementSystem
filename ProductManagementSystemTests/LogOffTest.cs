using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.User;
using System.Threading.Tasks;
using Xunit;

namespace ProductManagementSystemTests
{
    public class LogOffTest
    {
        [Fact]
        public async Task LogOff_ValidModel_RedirectsToLogin()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            var controller = new AccountController(authServiceMock.Object);

            // Act
            var result = await controller.LogOff(new RegisterModel()) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }

        [Fact]
        public async Task LogOff_CallsLogOffAsync()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            var controller = new AccountController(authServiceMock.Object);
            var model = new RegisterModel();

            // Act
            await controller.LogOff(model);

            // Assert
            authServiceMock.Verify(mock => mock.LoggOffAsync(model), Times.Once);
        }

        [Fact]
        public async Task LogOff_NullModel_RedirectsToLogin()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            var controller = new AccountController(authServiceMock.Object);
            var nullModel = new RegisterModel(); // Create a new instance

            // Act
            var result = await controller.LogOff(nullModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }
    }
}