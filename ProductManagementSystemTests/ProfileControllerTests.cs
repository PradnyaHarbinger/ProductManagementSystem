using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Profile;
using Xunit;

namespace ProductManagementSystemTests
{
    public class ProfileControllerTests
    {
        [Fact]
        public async Task Index_UserExists_ReturnsViewResult()
        {
            // Arrange
            var profileServiceMock = new Mock<IProfileService>();
            var controller = new HomeController(profileServiceMock.Object);

            // Mock a user with identity claims
            var userIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                // Add other claims as needed
            }, "mock");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            // Set the user for the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrincipal
                }
            };

            // Mock the ProfileService to return a user
            var currentUser = new UserModel
            {
                FirstName = "unit",
                LastName = "testing",
                UserName = "testuser",
                Role = "User",
                Email = "test@gmail.com"
            };

            profileServiceMock.Setup(mock => mock.GetUser(userPrincipal)).ReturnsAsync(currentUser);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<UserModel>(viewResult.Model);
        }

        [Fact]
        public async Task Index_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var profileServiceMock = new Mock<IProfileService>();
            var controller = new HomeController(profileServiceMock.Object);

            // Mock a user with identity claims
            var userIdentity = new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, "testuser"),
                // Add other claims as needed
            }, "mock");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            // Set the user for the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrincipal
                }
            };

            // Mock the ProfileService to return null for the user
            profileServiceMock.Setup(mock => mock.GetUser(userPrincipal)).ReturnsAsync((UserModel?)null!);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
