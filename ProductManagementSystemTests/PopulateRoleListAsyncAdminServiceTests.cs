using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using Xunit;

namespace ProductManagementSystemTests
{
    public class PopulateRoleListAsyncAdminServiceTests
    {
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

                // Load the SQL Server connection string from configuration
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json") // Adjust the path as needed
                    .Build();

                var connectionString = configuration.GetConnectionString("TestConnectionString1");

                // Create options for the DbContext using the SQL Server connection string
                var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                // Create the database and populate roles
                using (var context = new ApplicationDbContext(dbContextOptions))
                {
                    context.Database.EnsureCreated();
                    context.Roles.AddRange(roles);
                    context.SaveChanges();
                }

                var adminService = new AdminServices(null, new ApplicationDbContext(dbContextOptions));

                // Act
                await adminService.PopulateRoleListAsync(user);

                // Assert
                Assert.NotNull(user.RoleList);
                Assert.Equal(2, user.RoleList.Count());


                // Cleanup: Delete the test database
                using (var context = new ApplicationDbContext(dbContextOptions))
                {
                    context.Database.EnsureDeleted();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
