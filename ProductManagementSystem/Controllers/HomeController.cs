using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Profile;
using System.Diagnostics;

namespace ProductManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProfileService _profileService;

        public HomeController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _profileService.GetUser(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            return View(currentUser);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}