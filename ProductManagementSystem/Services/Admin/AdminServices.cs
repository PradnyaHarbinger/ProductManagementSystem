using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;

namespace ProductManagementSystem.Services.Admin
{
    public class AdminServices : IAdminServices
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminServices(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IdentityResult> AddUserAsync(AddUserModel model)
        {
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
                // Assign roles based on selected role
                var rolesToAdd = new List<string>();
                if (model.RoleSelected != null && model.RoleSelected.Length > 0 && model.RoleSelected == "Admin")
                {
                    rolesToAdd.Add("Admin");
                    rolesToAdd.Add("User");
                }
                else
                {
                    rolesToAdd.Add("User");
                }

                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            return result;
        }

        public List<SelectListItem> GetRoleList()
        {
            List<SelectListItem> listItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "User", Text = "User" }
        };
            return listItems;
        }

        public bool Delete(string userId)
        {
            var user = _db.UserModel.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return false; // User not found
            }

            _db.UserModel.Remove(user);
            _db.SaveChanges();
            return true; // User deleted successfully
        }

        /*public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }*/

        public async Task<IEnumerable<UserModel>> GetUserAsync()
        {
            var users = _db.UserModel.ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in users)
            {

                var role = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }

            }
            return users;
        }

        public UserModel GetUserForUpdate(string userId)
        {
            var user = _db.UserModel.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }
            var roleList = _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

            user.RoleList = roleList;

            return user;
        }

        public async Task<bool> UpdateUserAsync(UserModel user, string newRoleId)
        {
            var objFromDb = _db.UserModel.FirstOrDefault(u => u.Id == user.Id);
            if (objFromDb == null)
            {
                return false; // User not found
            }

            var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if (userRole != null)
            {
                var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);
            }

            // Add new role
            await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == newRoleId).Name);

            objFromDb.FirstName = user.FirstName;
            objFromDb.LastName = user.LastName;
            _db.SaveChanges();
            return true; // User updated successfully
        }

        public async Task<UserModel> GetUserForUpdateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) as UserModel;
            return user;
        }

        public async Task PopulateRoleListAsync(UserModel user)
        {
            // Fetch the list of roles
            user.RoleList = await _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            }).ToListAsync();
        }
    }
}
