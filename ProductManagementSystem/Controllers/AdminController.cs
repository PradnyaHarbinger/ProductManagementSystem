using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.Admin;

namespace ProductManagementSystem.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly IAdminServices _adminService;

        public AdminController(IAdminServices adminService)
        {
            _adminService = adminService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult GetUser()
        {
            var users = _adminService.GetUser();
            return View(users);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            var roleList = _adminService.GetRoleList();

            AddUserModel model = new()
            {
                RoleList = roleList
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AddUserModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminService.AddUserAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetUser");
                }

                AddErrors(result);
            }

            // Get role list from service and assign it to the model
            model.RoleList = _adminService.GetRoleList();

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }   
        }

        public IActionResult Update(string userId)
        {
            var userForUpdate = _adminService.GetUserForUpdate(userId);

            if (userForUpdate == null)
            {
                return NotFound(); // User not found
            }

            return View(userForUpdate);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var updated = await _adminService.UpdateUserAsync(user, user.RoleId);

                if (!updated)
                {
                    return NotFound();
                }

                TempData["Success"] = "User has been edited successfully.";
                return RedirectToAction(nameof(GetUser));
            }

            var userId = user.Id;
            user = await _adminService.GetUserForUpdateAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            await _adminService.PopulateRoleListAsync(user);

            return View(user);
        }



        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var deleted = _adminService.Delete(userId);
            if (!deleted)
            {
                TempData["Error"] = "User not found or unable to delete.";
                return RedirectToAction("GetUser");
            }

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction("GetUser");
        }
    }
}
