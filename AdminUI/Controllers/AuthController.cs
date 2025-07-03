using AdminUI.Models;
using AdminUI.Models.Authentication;
using AdminUI.Services;
using AdminUI.Services.AuthenServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminUI.Controllers
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
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Nếu đã đăng nhập, redirect về trang chính
            if (await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Login successful!";
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
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
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("ResetPassword");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            string email = HttpContext.Session.GetString("ResetEmail");
            string otpExpiration = HttpContext.Session.GetString("OTPExpiration");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otpExpiration))
            {
                TempData["ErrorMessage"] = "Invalid session. Please start the password reset process again.";
                return RedirectToAction("ForgotPassword");
            }

            if (DateTime.Parse(otpExpiration) < DateTime.UtcNow)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "OTP has expired. Please request a new one.";
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

            string storedEmail = HttpContext.Session.GetString("ResetEmail");
            string otpExpiration = HttpContext.Session.GetString("OTPExpiration");

            if (string.IsNullOrEmpty(storedEmail) || storedEmail != model.Email)
            {
                TempData["ErrorMessage"] = "Invalid session. Please start the password reset process again.";
                return RedirectToAction("ForgotPassword");
            }

            if (DateTime.Parse(otpExpiration) < DateTime.UtcNow)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "OTP has expired. Please request a new one.";
                return RedirectToAction("ForgotPassword");
            }

            var result = await _authService.ResetPasswordAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}