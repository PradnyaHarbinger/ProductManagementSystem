using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using System.Security.Claims;

namespace ProductManagementSystem.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserModel> GetUser(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            if (identityUser == null)
            {
                return new UserModel();
            }
            return (UserModel)identityUser;
        }
    }
}
