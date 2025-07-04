using AdminUI.Models.Profile;
using AdminUI.Services.AuthenServices;
using AdminUI.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminUI.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IUserService userService, IAuthService authService, ILogger<ProfileController> logger)
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _userService.GetProfileAsync();

            if (result.Success && result.Data != null)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load profile";
                return View(new ProfileViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _userService.GetProfileAsync();

            if (result.Success && result.Data != null)
            {
                var updateModel = new UpdateProfileViewModel
                {
                    FullName = result.Data.Fullname,
                    Address = result.Data.Address,
                    Phonenumber = result.Data.Phonenumber,
                    BirthDay = result.Data.BirthDay
                };
                return View(updateModel);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load profile";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProfileViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.UpdateProfileAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Profile updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update profile");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.ChangePasswordAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Password changed successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to change password");
                return View(model);
            }
        }
    }
}