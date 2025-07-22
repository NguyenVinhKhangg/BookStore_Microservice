using BookClient.Models.Cart;
using BookClient.Services.AuthServices;
using BookClient.Services.CartServices;
using Microsoft.AspNetCore.Mvc;

namespace BookClient.Controllers
{
    public class CartsController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly ILogger<CartsController> _logger;

        public CartsController(
            ICartService cartService,
            IAuthService authService,
            ILogger<CartsController> logger)
        {
            _cartService = cartService;
            _authService = authService;
            _logger = logger;
        }

        // GET: /Carts
        public async Task<IActionResult> Index()
        {
            // ✅ Check authentication for viewing cart
            if (!await _authService.IsAuthenticatedAsync())
            {
                TempData["ErrorMessage"] = "Please login to view your cart.";
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Carts") });
            }

            var result = await _cartService.GetUserCartAsync();

            if (result.Success && result.Data != null)
            {
                var summary = new CartSummaryViewModel
                {
                    Items = result.Data
                };

                return View(summary);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load cart";
                return View(new CartSummaryViewModel());
            }
        }

        // POST: /Carts/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(AddToCartViewModel model)
        {
            // ✅ Check authentication first
            if (!await _authService.IsAuthenticatedAsync())
            {
                _logger.LogInformation($"🔒 User not authenticated, redirecting to login for book {model.BookID}");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // For AJAX requests, return JSON with redirect URL
                    return Json(new
                    {
                        success = false,
                        message = "Please login to add items to cart",
                        requiresLogin = true,
                        redirectUrl = Url.Action("Login", "Auth", new
                        {
                            returnUrl = Request.Headers["Referer"].ToString()
                        })
                    });
                }

                // For regular form submissions, redirect to login with return URL
                TempData["InfoMessage"] = "Please login to add items to your cart.";
                return RedirectToAction("Login", "Auth", new
                {
                    returnUrl = Request.Headers["Referer"].ToString() ?? Url.Action("Index", "Home")
                });
            }

            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Invalid data" });
                }
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("Index", "Home");
            }

            var result = await _cartService.AddToCartAsync(model);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    totalItems = result.TotalItems
                });
            }

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: /Carts/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(UpdateCartItemViewModel model)
        {
            // ✅ Check authentication
            if (!await _authService.IsAuthenticatedAsync())
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please login to update cart",
                        requiresLogin = true,
                        redirectUrl = Url.Action("Login", "Auth")
                    });
                }
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            var result = await _cartService.UpdateCartItemAsync(model);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = result.Success,
                    message = result.Message
                });
            }

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: /Carts/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartId)
        {
            // ✅ Check authentication
            if (!await _authService.IsAuthenticatedAsync())
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please login to remove items from cart",
                        requiresLogin = true,
                        redirectUrl = Url.Action("Login", "Auth")
                    });
                }
                return RedirectToAction("Login", "Auth");
            }

            var result = await _cartService.RemoveFromCartAsync(cartId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = result.Success,
                    message = result.Message
                });
            }

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: /Carts/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            // ✅ Check authentication
            if (!await _authService.IsAuthenticatedAsync())
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please login to clear cart",
                        requiresLogin = true,
                        redirectUrl = Url.Action("Login", "Auth")
                    });
                }
                return RedirectToAction("Login", "Auth");
            }

            var result = await _cartService.ClearCartAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = result.Success,
                    message = result.Message
                });
            }

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: /Carts/GetCartCount (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            // ✅ Return 0 if not authenticated
            if (!await _authService.IsAuthenticatedAsync())
            {
                return Json(new { count = 0 });
            }

            var count = await _cartService.GetCartItemCountAsync();
            return Json(new { count = count });
        }
    }
}