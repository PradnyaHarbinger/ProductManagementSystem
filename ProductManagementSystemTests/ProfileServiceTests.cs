using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Profile;
using Xunit;

namespace ProductManagementSystemTests
{
    public class ProfileServiceTests : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _dbContext;

        public ProfileServiceTests()
        {
            // Configure the DbContext options to use an in-memory database
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ProfileServiceDb") // Use a unique database name
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

            var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, identityUser.UserName),
                new Claim(ClaimTypes.Email, identityUser.Email),
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
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
            // Cleanup: The in-memory database is automatically deleted when the DbContext is disposed
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
