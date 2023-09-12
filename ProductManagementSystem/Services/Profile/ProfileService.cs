using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using System.Security.Claims;

namespace ProductManagementSystem.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public ProfileService(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<UserModel> GetUser(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            if (identityUser == null)
            {
                return new UserModel();
            }

            var userData = _db.UserModel.FirstOrDefault(u => u.Id == identityUser.Id);


            var userModel = new UserModel
            {
                Id = identityUser.Id,
                Email = identityUser.Email,
                UserName = identityUser.UserName,
                FirstName = userData?.FirstName ?? "DefaultFirstName",
                LastName = userData?.LastName ?? "DefaultLastName"
            };
            return userModel;
        }
    }
}
