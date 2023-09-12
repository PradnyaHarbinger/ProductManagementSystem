using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using Xunit;

namespace ProductManagementSystemTests
{
    public class GetUserForUpdateAsyncAdminServiceTests
    {
        [Fact]
        public async Task GetUserForUpdateAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = "user123";
            var user = new UserModel
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe"
            };

            // Create a mock for UserManager<IdentityUser>
            var userManagerMock = MockUserManager(user);

            var adminService = new AdminServices(userManagerMock.Object, null);

            // Act
            var result = await adminService.GetUserForUpdateAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
        }

        [Fact]
        public async Task GetUserForUpdateAsync_UserDoesNotExist_ReturnsEmptyUser()
        {
            // Arrange
            var userId = "nonexistentUser";

            // Create a mock for UserManager<IdentityUser>
            var userManagerMock = MockUserManager(null); // Simulate a non-existent user

            var adminService = new AdminServices(userManagerMock.Object, null);

            // Act
            var result = await adminService.GetUserForUpdateAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(userId, result.Id);
            Assert.Empty(result.FirstName);
            Assert.Empty(result.LastName);
        }

        // Helper method to create a mock for UserManager<IdentityUser>
        private static Mock<UserManager<IdentityUser>> MockUserManager(UserModel user)
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Mock FindByIdAsync to return the provided user or null
            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            return userManagerMock;
        }
    }
}
