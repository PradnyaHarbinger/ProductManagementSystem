using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using System.Linq;
using Xunit;

namespace ProductManagementSystemTests
{
    public class DeleteAdminServiceTests
    {
        private readonly IConfigurationRoot _configuration;

        public DeleteAdminServiceTests()
        {
            // Load your appsettings.json or any configuration file that includes the connection string
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        [Fact]
        public void Delete_UserExists_ReturnsTrue()
        {
            // Arrange
            var userId = "existingUserId";
            var user = new UserModel { Id = userId };
            var userManager = MockUserManager();

            var connectionString = _configuration.GetConnectionString("TestConnectionString");
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString) // Use your SQL Server connection string here
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated(); // Ensure the database is created
                context.UserModel.Add(user);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var adminService = new AdminServices(userManager.Object, context);

                // Act
                var result = adminService.Delete(userId);

                // Assert
                Assert.True(result); // User should be deleted successfully
            }
        }

        [Fact]
        public void Delete_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var userId = "nonExistingUserId";
            var userManager = MockUserManager();

            var connectionString = _configuration.GetConnectionString("TestConnectionString");
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString) // Use your SQL Server connection string here
                .Options;

            // Create a real instance of ApplicationDbContext with a real database connection
            using (var context = new ApplicationDbContext(options))
            {
                var adminService = new AdminServices(userManager.Object, context);

                // Act
                var result = adminService.Delete(userId);

                // Assert
                Assert.False(result); // User should not be found and therefore not deleted
            }
        }

        private Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }
    }
}
