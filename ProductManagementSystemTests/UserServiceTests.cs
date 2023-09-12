using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using Moq;
using ProductManagementSystem.Models.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductManagementSystemTests
{
    public class UserServiceTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

        public UserServiceTests()
        {
            // Arrange: Setup the services
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json") // Use your test configuration file
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(configuration.GetConnectionString("TestConnectionString"), options =>
            {
                options.EnableRetryOnFailure();
            });

            _dbContext = new ApplicationDbContext(optionsBuilder.Options);

            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            _serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("TestConnectionString"));
                })
                .AddIdentity<IdentityUser, IdentityRole>() // Use the default IdentityUser and IdentityRole
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .Services
                .AddSingleton(_userManagerMock.Object)
                .AddSingleton(_roleManagerMock.Object)
                .BuildServiceProvider();
        }

        [Fact]
        public async Task AddUserAsync_Success()
        {
            try
            {
                // Arrange: Initialize the AdminServices with Mock UserManager
                var adminServices = new AdminServices(_userManagerMock.Object, _dbContext);

                // Setup UserManager Mock to return success
                _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);

                // Create a sample addUserModel
                var addUserModel = new AddUserModel
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "johndoe@test.com",
                    Password = "Password@123",
                    RoleSelected = "Admin"
                };

                // Act: Create the user
                var result = await adminServices.AddUserAsync(addUserModel);

                // Assert
                Assert.True(result.Succeeded);

                // Verify that the user was created with the expected email
                _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), addUserModel.Password), Times.Once);
                _userManagerMock.Verify(um => um.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>()), Times.Once);

                // Verify any other assertions you need
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [Fact]
        public async Task AddUserAsync_Failure_InvalidRole()
        {
            // Arrange: Initialize the AdminServices with Mock UserManager
            var adminServices = new AdminServices(_userManagerMock.Object, _dbContext);

            // Setup UserManager Mock to return failure
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "InvalidRole", Description = "Invalid role specified" }));

            // Create a sample addUserModel with an invalid role
            var addUserModel = new AddUserModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "Password123",
                RoleSelected = "InvalidRole"
            };

            // Act: Attempt to create the user with an invalid role
            var result = await adminServices.AddUserAsync(addUserModel);

            // Assert: Expect failure due to an invalid role
            Assert.False(result.Succeeded);
        }

        [Fact]
        public void GetRoleList_ReturnsExpectedList()
        {
            // Arrange
            var adminServices = new AdminServices(_userManagerMock.Object, _dbContext);

            // Act
            var result = adminServices.GetRoleList();

            // Assert
            // Check that the result is not null
            Assert.NotNull(result);

            // Check that the result is a list of SelectListItem
            Assert.IsType<List<SelectListItem>>(result);

            // Check that the result contains the expected items
            Assert.Collection(
                result,
                item => Assert.Equal("Admin", item.Value),
                item => Assert.Equal("User", item.Value)
            );
        }

        
        // Cleanup the test database after all tests
        public void Cleanup_TestDatabase()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
