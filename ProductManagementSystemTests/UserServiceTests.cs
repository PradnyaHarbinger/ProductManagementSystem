using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
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
    public class UserServiceTests : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

        public UserServiceTests()
        {
            // Arrange: Setup the services
            var services = new ServiceCollection();

            // Configure the DbContext to use an in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "UserServiceTestDb");
            });

            // Add Identity services with in-memory database
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            services.AddSingleton(_userManagerMock.Object);
            services.AddSingleton(_roleManagerMock.Object);

            _serviceProvider = services.BuildServiceProvider();

            // Create a new instance of the ApplicationDbContext to be used during cleanup
            _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
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

        public void Dispose()
        {
            // Cleanup: Dispose of the DbContext and Service Provider
            _dbContext.Dispose();
            _serviceProvider.Dispose();
        }
    }
}
