using ProductManagementSystem.Models;
using System.Security.Claims;

namespace ProductManagementSystem.Services.Profile
{
    public interface IProfileService
    {
        Task<UserModel> GetUser(ClaimsPrincipal user);
    }
}
