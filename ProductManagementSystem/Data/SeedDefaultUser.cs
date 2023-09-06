using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Data
{
    public class SeedDefaultUser
    {
        private SeedDefaultUser() { }
        public static async Task SeedSuperAdmin(UserManager<IdentityUser> userManager,
                                                string Password)
        {
            var SuperAdmin = new UserModel
            {
                FirstName = "Pradnya",
                LastName = "K",
                UserName = "superadmin@gmail.com",
                Email = "superadmin@gmail.com",
                EmailConfirmed = true,
            };
            SuperAdmin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(SuperAdmin, Password);

            if (userManager.Users.All(u => u.Id != SuperAdmin.Id))
            {
                var user = await userManager.FindByEmailAsync(SuperAdmin.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(SuperAdmin);
                    await userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");
                    await userManager.AddToRoleAsync(SuperAdmin, "Admin");
                    await userManager.AddToRoleAsync(SuperAdmin, "User");

                }
            }

        }
    }
}
