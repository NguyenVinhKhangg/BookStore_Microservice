using BookClient.Models.Authentication;
using BookClient.Services.AuthServices;
using Microsoft.AspNetCore.Mvc;

namespace BookClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập, redirect về Home
            if (_authService.IsAuthenticatedAsync().Result)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result.Success)
            {
                _logger.LogInformation($"User {model.Email} logged in successfully");
                TempData["SuccessMessage"] = "Welcome! You have successfully logged in.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogWarning($"Failed login attempt for user {model.Email}: {result.Message}");
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Nếu đã đăng nhập, redirect về Home
            if (_authService.IsAuthenticatedAsync().Result)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterAsync(model);

            if (result.Success)
            {
                _logger.LogInformation($"User {model.Email} registered successfully");
                TempData["SuccessMessage"] = "Registration successful! You can now login with your credentials.";
                return RedirectToAction("Login");
            }
            else
            {
                _logger.LogWarning($"Failed registration attempt for user {model.Email}: {result.Message}");
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.ForgotPasswordAsync(model);

            if (result.Success)
            {
                _logger.LogInformation($"Password reset request sent for user {model.Email}");
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("ResetPassword", new { email = model.Email });
            }
            else
            {
                _logger.LogWarning($"Failed forgot password attempt for user {model.Email}: {result.Message}");
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.ResetPasswordAsync(model);

            if (result.Success)
            {
                _logger.LogInformation($"Password reset successful for user {model.Email}");
                TempData["SuccessMessage"] = "Password reset successful! You can now login with your new password.";
                return RedirectToAction("Login");
            }
            else
            {
                _logger.LogWarning($"Failed password reset attempt for user {model.Email}: {result.Message}");
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogoutAsync();

            if (result)
            {
                _logger.LogInformation("User logged out successfully");
                TempData["SuccessMessage"] = "You have been logged out successfully.";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}