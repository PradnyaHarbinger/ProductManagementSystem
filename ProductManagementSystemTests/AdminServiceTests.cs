using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using Xunit;

namespace ProductManagementSystemTests
{
    public class AdminServiceTests : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;

        public AdminServiceTests()
        {
            // Load configuration from appsettings.json
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
                
            // Setup the database context
            var connectionString = _configuration.GetConnectionString("TestConnectionString2");
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;
            _context = new ApplicationDbContext(options);

            // Ensure the database is created and migrations are applied
            _context.Database.EnsureCreated();

            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        }

        public void Dispose()
        {
            // Cleanup: Delete the test database
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }


        [Fact]
        public void GetUser_ReturnsUsersWithRoles()
        {
            // Test code for GetUser
            // Set up the database with known test data
            var user1 = new UserModel { Id = "user1" };
            var user2 = new UserModel { Id = "user2" };
            _context.Users.Add(user1);
            _context.Users.Add(user2);
            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = "user1", RoleId = "role1" });
            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = "user2", RoleId = "role2" });
            _context.Roles.Add(new IdentityRole { Id = "role1", Name = "Admin" });
            _context.Roles.Add(new IdentityRole { Id = "role2", Name = "User" });
            _context.SaveChanges();

            var adminService = new AdminServices(_userManagerMock.Object, _context);

            // Act
            var result = adminService.GetUser();

            // Assert
            Assert.NotNull(result);
            var userList = new List<UserModel>(result);
            Assert.Equal(2, userList.Count);

            // Verify that roles are correctly assigned to users
            Assert.Equal("Admin", userList[0].Role);
            Assert.Equal("User", userList[1].Role);

            // Delete the added data
            _context.Users.Remove(user1);
            _context.Users.Remove(user2);
            _context.UserRoles.RemoveRange(_context.UserRoles);
            _context.Roles.RemoveRange(_context.Roles);
            _context.SaveChanges();
        }

        [Fact]
        public void GetUserForUpdate_UserExists_ReturnsUserWithRoleList()
        {
            // Test code for GetUserForUpdate when the user exists
            var userId = "user123";
            var user = new UserModel
            {
                Id = userId,
                // Initialize other user properties as needed
            };

            var roleList = new List<IdentityRole>
            {
                new IdentityRole { Id = "role1", Name = "Admin" },
                new IdentityRole { Id = "role2", Name = "User" },
            };

            _context.Users.Add(user);
            _context.Roles.AddRange(roleList);
            _context.SaveChanges();

            var adminService = new AdminServices(_userManagerMock.Object, _context);

            // Act
            var result = adminService.GetUserForUpdate(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.NotNull(result.RoleList);
            Assert.Equal(2, result.RoleList.Count());
        }

        [Fact]
        public void GetUserForUpdate_UserDoesNotExist_ReturnsEmptyUser()
        {
            // Test code for GetUserForUpdate when the user doesn't exist
            var userId = "nonexistentUser";

            var adminService = new AdminServices(_userManagerMock.Object, _context);

            // Act
            var result = adminService.GetUserForUpdate(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(userId, result.Id);
        }

    }
}
