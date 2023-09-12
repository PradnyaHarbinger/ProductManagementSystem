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
            List<SelectListItem> listItems = new()
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

        public IEnumerable<UserModel> GetUser()
        {
            var users = _db.UserModel.ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            var rolesById = roles.ToDictionary(r => r.Id, r => r);

            foreach (var user in users)
            {
                var role = userRoles.Find(ur => ur.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    if (rolesById.TryGetValue(role.RoleId, out var userRole))
                    {
                        user.Role = userRole.Name;
                    }
                    else
                    {
                        user.Role = "None";
                    }
                }
            }

            return users;
        }


        public UserModel GetUserForUpdate(string userId)
        {
            var user = _db.UserModel.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return new UserModel();
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

            var role = _db.Roles.FirstOrDefault(u => u.Id == newRoleId);
            if (role != null)
            {
                await _userManager.AddToRoleAsync(objFromDb, role.Name);
            }

            objFromDb.FirstName = user.FirstName;
            objFromDb.LastName = user.LastName;
            _db.SaveChanges();
            return true; // User updated successfully
        }

        public async Task<UserModel> GetUserForUpdateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) as UserModel;
            if(user == null) {
                return new UserModel(); 
            }
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
