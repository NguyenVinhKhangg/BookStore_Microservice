using AdminUI.Models.User;
using AdminUI.Services.AuthenServices;
using AdminUI.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdminUI.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IUserService _userManagementService;
        private readonly IAuthService _authService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserService userManagementService,
            IAuthService authService,
            ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _authService = authService;
            _logger = logger;
        }

        // ✅ ADDED: Helper method to check if user has admin access
        private bool HasAdminAccess()
        {
            var userInfo = HttpContext.Session.GetString("UserInfo");
            if (string.IsNullOrEmpty(userInfo))
                return false;

            try
            {
                var user = JsonSerializer.Deserialize<AdminUI.Models.Authentication.UserModel>(userInfo);
                // Only Admin (RoleId = 1) can access User Management
                return user != null && (user.RoleId == 1 || user.RoleName?.ToLower() == "admin");
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(UserSearchFilterViewModel filter)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can access User Management.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManagementService.GetUsersAsync(filter);

            ViewBag.Filter = filter;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.PageSize = filter.PageSize;
            ViewBag.CurrentPage = filter.Page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / filter.PageSize);

            if (result.Success)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load users";
                return View(new List<UserManagementViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can create users.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can create users.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userManagementService.CreateUserAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "User created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create user");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can edit users.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManagementService.GetUserByIdAsync(id);

            if (result.Success && result.Data != null)
            {
                var updateModel = new UpdateUserViewModel
                {
                    UserID = result.Data.UserID,
                    FullName = result.Data.FullName,
                    Email = result.Data.Email,
                    Phonenumber = result.Data.PhoneNumber,
                    Address = result.Data.Address,
                    BirthDay = result.Data.BirthDay,
                    RoleId = result.Data.RoleId
                };
                return View(updateModel);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "User not found";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can edit users.";
                return RedirectToAction("Index", "Home");
            }

            // Remove password from ModelState if empty
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.Remove("Password");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userManagementService.UpdateUserAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "User updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update user");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetail(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return Unauthorized("Authentication required");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                return Forbid("Access denied. Only administrators can view user details.");
            }

            try
            {
                _logger.LogInformation($"Getting user details for ID: {id}");

                var result = await _userManagementService.GetUserByIdAsync(id);

                if (result.Success && result.Data != null)
                {
                    return PartialView("_UserDetailPartial", result.Data);
                }
                else
                {
                    return PartialView("_UserDetailPartial", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in GetUserDetail for ID: {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can activate users.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManagementService.ActivateUserAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "User activated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to activate user";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            // ✅ ADDED: Check admin access
            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can deactivate users.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManagementService.DeactivateUserAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "User deactivated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to deactivate user";
            }

            return RedirectToAction("Index");
        }
    }
}