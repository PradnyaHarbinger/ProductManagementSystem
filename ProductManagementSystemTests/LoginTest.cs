using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.Admin;
using ProductManagementSystem.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ProductManagementSystemTests
{
    public class LoginTest
    {
        [Fact]
        public void Login_ReturnsViewResult()
        {
            // Arrange
            var authServicesMock = new Mock<IAuthServices>();
            var controller = new AccountController(authServicesMock.Object);

            // Act
            var result = controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async Task Login_ValidCredentials_RedirectsToHomePage()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            authServiceMock.Setup(mock => mock.LoginAsync(It.IsAny<LoginModel>()))
                           .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success); // Use SignInResult.Success for a valid login

            var controller = new AccountController(authServiceMock.Object);
            var validLoginModel = new LoginModel
            {
                // Set valid credentials
                Email = "demouser@gmail.com",
                Password = "User@123"
            };

            // Act
            var result = await controller.Login(validLoginModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName); // Redirects to the home page
        }


        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            authServiceMock.Setup(mock => mock.LoginAsync(It.IsAny<LoginModel>()))
                           .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Use SignInResult.Failed for an invalid login

            var controller = new AccountController(authServiceMock.Object);
            var invalidLoginModel = new LoginModel
            {
                // Set invalid credentials
                Email = "demouser@gmail.com",
                Password = "Pass@123"
            };

            // Act
            var result = await controller.Login(invalidLoginModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e =>
                e.ErrorMessage == "Invalid credentials");
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            authServiceMock.Setup(mock => mock.LoginAsync(It.IsAny<LoginModel>()))
                           .ReturnsAsync(SignInResult.Failed);

            var controller = new AccountController(authServiceMock.Object);
            var notFoundLoginModel = new LoginModel
            {
                // Set credentials for a user that doesn't exist
                Email = "nonexistentuser",
                Password = "Pass@123"
            };

            // Act
            var result = await controller.Login(notFoundLoginModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e =>
                e.ErrorMessage == "Invalid credentials");
        }



    }
}
