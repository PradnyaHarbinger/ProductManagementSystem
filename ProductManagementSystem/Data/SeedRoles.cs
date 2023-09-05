using Microsoft.AspNetCore.Identity;

namespace ProductManagementSystem.Data
{
    public class SeedRoles
    {
        public static async Task SeedRoleAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }
        }
    }
}
