using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.Profile;
using Xunit;

namespace ProductManagementSystemTests
{
    public class ProfileServiceTests : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly string _connectionString;
        private readonly ApplicationDbContext _dbContext;

        public ProfileServiceTests()
        {
            // Set the connection string to your local SQL Server (LocalDB) instance
            _connectionString = "Server=(localdb)\\mssqllocaldb;Database=ProfileDb;Trusted_Connection=True;MultipleActiveResultSets=true";

            // Configure the DbContext options to use SQL Server (LocalDB)
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            // Initialize the database with schema and test data
            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.EnsureCreated();
            }

            // Create a new instance of the ApplicationDbContext to be used during cleanup
            _dbContext = new ApplicationDbContext(_options);
        }

        [Fact]
        public async Task GetUser_UserExists_ReturnsUserModel()
        {
            // Arrange
            var userManagerMock = MockUserManager();
            var profileService = new ProfileService(userManagerMock.Object, new ApplicationDbContext(_options));

            var identityUser = new IdentityUser
            {
                Id = "user123",
                Email = "user@example.com",
                UserName = "user@example.com"
            };

            var userClaimsPrincipal = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, identityUser.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, identityUser.Email),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, identityUser.Id),
            }));

            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(identityUser);

            using (var context = new ApplicationDbContext(_options))
            {
                context.UserModel.Add(new UserModel
                {
                    Id = identityUser.Id,
                    FirstName = "John",
                    LastName = "Doe"
                });
                context.SaveChanges();
            }

            // Act
            var result = await profileService.GetUser(userClaimsPrincipal);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(identityUser.Id, result.Id);
            Assert.Equal(identityUser.Email, result.Email);
            Assert.Equal(identityUser.UserName, result.UserName);
        }

        public void Dispose()
        {
            // Cleanup: Delete the test database
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        private static Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var userManager = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Configure UserManager methods as needed for your test

            return userManager;
        }
    }
}
