using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.User;
using Xunit;

namespace ProductManagementSystemTests
{
    public class AuthServicesTests
    {
        [Fact]
        public async Task RegisterAsync_NewUser_SuccessfulRegistration()
        {
            // Arrange
            var userManagerMock = MockUserManager();
            var signInManagerMock = MockSignInManager();
            var authService = new AuthServices(userManagerMock.Object, signInManagerMock.Object);

            var registerModel = new RegisterModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "P@ssw0rd"
            };

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) => null);
            userManagerMock.Setup(m => m.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await authService.RegisterAsync(registerModel);

            // Assert
            Assert.True(result.Succeeded);
            userManagerMock.Verify(
                m => m.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()), Times.Once);
            signInManagerMock.Verify(
                m => m.SignInAsync(It.IsAny<UserModel>(), false, null), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ExistingUser_ReturnsError()
        {
            // Arrange
            var userManagerMock = MockUserManager();
            var signInManagerMock = MockSignInManager();
            var authService = new AuthServices(userManagerMock.Object, signInManagerMock.Object);

            var registerModel = new RegisterModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "P@ssw0rd"
            };

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserModel());

            // Act
            var result = await authService.RegisterAsync(registerModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, error => error.Description == "User with this email already exists.");
            userManagerMock.Verify(
                m => m.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()), Times.Never);
            signInManagerMock.Verify(
                m => m.SignInAsync(It.IsAny<UserModel>(), false, null), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = MockUserManager();
            var signInManagerMock = MockSignInManager();
            var authService = new AuthServices(userManagerMock.Object, signInManagerMock.Object);

            var loginModel = new LoginModel
            {
                Email = "john@example.com",
                Password = "P@ssw0rd",
                RememberMe = false
            };

            signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), true))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await authService.LoginAsync(loginModel);

            // Assert
            Assert.Equal(SignInResult.Success, result);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = MockUserManager();
            var signInManagerMock = MockSignInManager();
            var authService = new AuthServices(userManagerMock.Object, signInManagerMock.Object);

            var loginModel = new LoginModel
            {
                Email = "john@example.com",
                Password = "WrongPassword",
                RememberMe = false
            };

            signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), true))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await authService.LoginAsync(loginModel);

            // Assert
            Assert.Equal(SignInResult.Failed, result);
        }

        [Fact]
        public async Task LoggOffAsync_LogsUserOff()
        {
            // Arrange
            var userManagerMock = MockUserManager();
            var signInManagerMock = MockSignInManager();
            var authService = new AuthServices(userManagerMock.Object, signInManagerMock.Object);

            // Act
            await authService.LoggOffAsync(new RegisterModel());

            // Assert
            signInManagerMock.Verify(m => m.SignOutAsync(), Times.Once);
        }

        private static Mock<UserManager<IdentityUser>> MockUserManager()
        {
            return new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        }

        private static Mock<SignInManager<IdentityUser>> MockSignInManager()
        {
            return new Mock<SignInManager<IdentityUser>>(
                MockUserManager().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                null,
                null,
                null,
                null);
        }
    }
}
