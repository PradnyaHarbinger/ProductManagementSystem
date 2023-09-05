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
        private readonly ILogger<HomeController> _logger;
        private readonly IProfileService _profileService;

        public HomeController(ILogger<HomeController> logger,
                                IProfileService profileService)
        {
            _logger = logger;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}