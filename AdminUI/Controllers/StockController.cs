using AdminUI.Models.Stock;
using AdminUI.Services.AuthenServices;
using AdminUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdminUI.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IAuthService _authService;
        private readonly ILogger<StockController> _logger;

        public StockController(
            IStockService stockService,
            IAuthService authService,
            ILogger<StockController> logger)
        {
            _stockService = stockService;
            _authService = authService;
            _logger = logger;
        }

        // Helper method to check if user has access to stock management
        private bool HasStockAccess()
        {
            var userInfo = HttpContext.Session.GetString("UserInfo");
            if (string.IsNullOrEmpty(userInfo))
                return false;

            try
            {
                var user = JsonSerializer.Deserialize<AdminUI.Models.Authentication.UserModel>(userInfo);
                // Both Admin (RoleId = 1) and Staff (RoleId = 3) can access Stock Management
                return user != null && (user.RoleId == 1 || user.RoleId == 3 || 
                                       user.RoleName?.ToLower() == "admin" || user.RoleName?.ToLower() == "staff");
            }
            catch
            {
                return false;
            }
        }

        // Helper method to check if user is admin
        private bool IsAdmin()
        {
            var userInfo = HttpContext.Session.GetString("UserInfo");
            if (string.IsNullOrEmpty(userInfo))
                return false;

            try
            {
                var user = JsonSerializer.Deserialize<AdminUI.Models.Authentication.UserModel>(userInfo);
                return user != null && (user.RoleId == 1 || user.RoleName?.ToLower() == "admin");
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(StockSearchFilterViewModel filter)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasStockAccess())
            {
                TempData["ErrorMessage"] = "Access denied. You don't have permission to access Stock Management.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _stockService.GetTransactionsAsync(filter);

            ViewBag.Filter = filter;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.PageSize = filter.PageSize;
            ViewBag.CurrentPage = filter.Page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / filter.PageSize);
            ViewBag.IsAdmin = IsAdmin();

            if (result.Success)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load transactions";
                return View(new List<StockTransactionViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasStockAccess())
            {
                TempData["ErrorMessage"] = "Access denied. You don't have permission to create transactions.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStockTransactionViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasStockAccess())
            {
                TempData["ErrorMessage"] = "Access denied. You don't have permission to create transactions.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _stockService.CreateTransactionAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Transaction created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create transaction");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasStockAccess())
            {
                TempData["ErrorMessage"] = "Access denied. You don't have permission to view transaction details.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _stockService.GetTransactionByIdAsync(id);

            if (result.Success && result.Data != null)
            {
                ViewBag.IsAdmin = IsAdmin();
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Transaction not found";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, UpdateTransactionStatusViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can update transaction status.";
                return RedirectToAction("Details", new { id });
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input data.";
                return RedirectToAction("Details", new { id });
            }

            var result = await _stockService.UpdateTransactionStatusAsync(id, model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Transaction status updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to update transaction status";
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can delete transactions.";
                return RedirectToAction("Index");
            }

            var result = await _stockService.DeleteTransactionAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Transaction deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to delete transaction";
            }

            return RedirectToAction("Index");
        }
    }
}