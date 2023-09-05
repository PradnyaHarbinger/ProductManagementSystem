using Microsoft.AspNetCore.Identity;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using System.Security.Claims;

namespace ProductManagementSystem.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileService(ApplicationDbContext db,
                                UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<UserModel> GetUser(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            return identityUser as UserModel;
        }
    }
}
