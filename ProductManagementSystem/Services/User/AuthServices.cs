using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;

namespace ProductManagementSystem.Services.User
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthServices(UserManager<IdentityUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            model.Email = model.Email.ToLower();
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User with this email already exists." });
            }

            var user = new UserModel
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
            return result;
        }

        public async Task LoggOffAsync(RegisterModel model)
        {
            await _signInManager.SignOutAsync();
        }
        

    }
}
