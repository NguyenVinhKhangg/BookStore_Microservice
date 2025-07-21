using AdminUI.Models.Category;
using AdminUI.Services.AuthenServices;
using AdminUI.Services.CategoryServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdminUI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IAuthService _authService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryService categoryService,
            IAuthService authService,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _authService = authService;
            _logger = logger;
        }

        // ✅ Helper method to check if user has admin access
        private bool HasAdminAccess()
        {
            var userInfo = HttpContext.Session.GetString("UserInfo");
            if (string.IsNullOrEmpty(userInfo))
                return false;

            try
            {
                var user = JsonSerializer.Deserialize<AdminUI.Models.Authentication.UserModel>(userInfo);
                // Only Admin (RoleId = 1) can access Category Management
                return user != null && (user.RoleId == 1 || user.RoleName?.ToLower() == "admin");
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(CategorySearchFilterViewModel filter)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can access Category Management.";
                return RedirectToAction("Index", "Home");
            }

            // ✅ Ensure filter is properly initialized
            if (filter == null)
            {
                filter = new CategorySearchFilterViewModel();
            }

            // ✅ Ensure proper default values
            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 10;

            _logger.LogInformation($"=== CATEGORY MANAGEMENT INDEX ===");
            _logger.LogInformation($"Filter: SearchTerm='{filter.SearchTerm}', StatusFilter={filter.StatusFilter}");
            _logger.LogInformation($"Pagination: Page={filter.Page}, PageSize={filter.PageSize}");

            var result = await _categoryService.GetCategoriesAsync(filter);

            _logger.LogInformation($"Service Result: Success={result.Success}, TotalCount={result.TotalCount}, DataCount={result.Data?.Count() ?? 0}");

            // ✅ Calculate proper pagination values
            var totalCount = result.TotalCount;
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            ViewBag.Filter = filter;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = filter.PageSize;
            ViewBag.CurrentPage = filter.Page;
            ViewBag.TotalPages = totalPages;

            _logger.LogInformation($"ViewBag: TotalCount={totalCount}, CurrentPage={filter.Page}, TotalPages={totalPages}, PageSize={filter.PageSize}");

            if (result.Success)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load categories";
                return View(new List<CategoryViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can create categories.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can create categories.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _categoryService.CreateCategoryAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Category created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create category");
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

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can edit categories.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (result.Success && result.Data != null)
            {
                var updateModel = new UpdateCategoryViewModel
                {
                    CategoryID = result.Data.CategoryID,
                    Name = result.Data.Name,
                    Description = result.Data.Description,
                    IsActive = result.Data.IsActive
                };
                return View(updateModel);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Category not found";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoryViewModel model)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can edit categories.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _categoryService.UpdateCategoryAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Category updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update category");
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

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can view category details.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (result.Success && result.Data != null)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Category not found";
                return RedirectToAction("Index");
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

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can activate categories.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _categoryService.ActivateCategoryAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Category activated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to activate category";
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

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can deactivate categories.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _categoryService.DeactivateCategoryAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Category deactivated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to deactivate category";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!HasAdminAccess())
            {
                TempData["ErrorMessage"] = "Access denied. Only administrators can delete categories.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _categoryService.DeleteCategoryAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Category deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to delete category";
            }

            return RedirectToAction("Index");
        }

        // ✅ AJAX endpoint for category details modal
        [HttpGet]
        public async Task<IActionResult> GetCategoryDetail(int id)
        {
            if (!await _authService.IsAuthenticatedAsync())
            {
                return Unauthorized("Authentication required");
            }

            if (!HasAdminAccess())
            {
                return Forbid("Access denied. Only administrators can view category details.");
            }

            try
            {
                _logger.LogInformation($"Getting category details for ID: {id}");

                var result = await _categoryService.GetCategoryByIdAsync(id);

                if (result.Success && result.Data != null)
                {
                    return PartialView("_CategoryDetailPartial", result.Data);
                }
                else
                {
                    return PartialView("_CategoryDetailPartial", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in GetCategoryDetail for ID: {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }
    }
}