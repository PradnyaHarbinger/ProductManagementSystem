using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;

namespace ProductManagementSystem.Services.Admin
{
    public interface IAdminServices
    {
        Task<IdentityResult> AddUserAsync(AddUserModel model);
        List<SelectListItem> GetRoleList();
        Task<IEnumerable<UserModel>> GetUserAsync();
        Task<bool> UpdateUserAsync(UserModel model, string newRoleId);
        Task<UserModel> GetUserForUpdateAsync(string userId);
        Task PopulateRoleListAsync(UserModel user);
        UserModel GetUserForUpdate(string userId);
        bool Delete(string userId);
    }
}
