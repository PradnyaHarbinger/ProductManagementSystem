using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using Xunit;

namespace ProductManagementSystemTests
{
    public class PopulateRoleListAsyncAdminServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public PopulateRoleListAsyncAdminServiceTests()
        {
            // Set up the in-memory database context
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PopulateRoleListAdminServiceDb")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new ApplicationDbContext(options);
        }

        public void Dispose()
        {
            // Dispose of the in-memory database context
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task PopulateRoleListAsync_FetchesRolesAndPopulatesRoleList()
        {
            try
            {
                // Arrange
                var user = new UserModel { Id = "user123" };
                var roles = new List<IdentityRole>
                {
                    new IdentityRole { Id = "role1", Name = "Admin" },
                    new IdentityRole { Id = "role2", Name = "User" },
                };

                // Add roles to the in-memory database
                _context.Roles.AddRange(roles);
                _context.SaveChanges();

                var adminService = new AdminServices(null, _context);

                // Act
                await adminService.PopulateRoleListAsync(user);

                // Assert
                Assert.NotNull(user.RoleList);
                Assert.Equal(2, user.RoleList.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
