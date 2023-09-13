using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using Xunit;

namespace ProductManagementSystemTests
{
    public class DeleteAdminServiceTests : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly ApplicationDbContext _context;

        public DeleteAdminServiceTests()
        {
            // Set up DbContextOptions for the in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteAdminServiceTestDb") // Use a unique database name
                .Options;

            // Create a new instance of the ApplicationDbContext with in-memory database
            _context = new ApplicationDbContext(_dbContextOptions);
        }

        public void Dispose()
        {
            // Dispose of the context to release in-memory database resources
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Delete_UserExists_ReturnsTrue()
        {
            // Arrange
            var userId = "existingUserId";
            var user = new UserModel { Id = userId };
            var userManager = MockUserManager();

            _context.Database.EnsureCreated(); // Ensure the database is created
            _context.UserModel.Add(user);
            _context.SaveChanges();

            var adminService = new AdminServices(userManager.Object, _context);

            // Act
            var result = adminService.Delete(userId);

            // Assert
            Assert.True(result); // User should be deleted successfully
        }

        [Fact]
        public void Delete_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var userId = "nonExistingUserId";
            var userManager = MockUserManager();

            var adminService = new AdminServices(userManager.Object, _context);

            // Act
            var result = adminService.Delete(userId);

            // Assert
            Assert.False(result); // User should not be found and therefore not deleted
        }

        private static Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            // Set up methods on userManager as needed for your tests
            userManager.Setup(u => u.DeleteAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Add more setup for methods you need in your tests

            return userManager;
        }
    }
}