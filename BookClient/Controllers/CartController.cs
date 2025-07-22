using BookClient.Models;
using BookClient.Models.Cart;
using BookClient.Services.CartServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookClient.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userInfo = HttpContext.Session.GetString("UserInfo");
            if (string.IsNullOrEmpty(userInfo))
            {
                return null;
            }

            try
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<BookClient.Models.Authentication.UserModel>(userInfo);
                return user?.UserID;
            }
            catch
            {
                return null;
            }
        }

        private bool IsAuthenticated()
        {
            return GetCurrentUserId().HasValue;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Cart") });
            }

            try
            {
                var userId = GetCurrentUserId().Value;
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải giỏ hàng.";
                return View(new CartViewModel());
            }
        }

        // POST: /Cart/AddToCart (AJAX)
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                // ✅ Kiểm tra authentication trước khi thêm vào cart
                if (!IsAuthenticated())
                {
                    return Json(new
                    {
                        success = false,
                        requiresLogin = true,
                        message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.",
                        redirectUrl = Url.Action("Login", "Auth", new { returnUrl = Url.Action("Index", "Home") })
                    });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
                }

                var userId = GetCurrentUserId().Value;
                var cartItem = await _cartService.AddToCartAsync(userId, request);

                // ✅ Lấy cart count mới
                var cartCount = await _cartService.GetCartItemCountAsync(userId);

                return Json(new
                {
                    success = true,
                    message = "Đã thêm sách vào giỏ hàng!",
                    cartItem = cartItem,
                    totalItems = cartCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return Json(new
                {
                    success = false,
                    message = ex.Message.Contains("Failed to add") ? "Không thể thêm sách vào giỏ hàng. Vui lòng thử lại." : "Có lỗi xảy ra khi thêm sách vào giỏ hàng."
                });
            }
        }

        // POST: /Cart/AddToCartForm (Form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCartForm(int bookId, int quantity, decimal price)
        {
            try
            {
                // ✅ Kiểm tra authentication cho form submission
                if (!IsAuthenticated())
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                    return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Details", "Home", new { id = bookId }) });
                }

                var request = new AddToCartRequest
                {
                    BookID = bookId,
                    Quantity = quantity,
                    Price = price
                };

                var userId = GetCurrentUserId().Value;
                await _cartService.AddToCartAsync(userId, request);

                TempData["SuccessMessage"] = "Đã thêm sách vào giỏ hàng!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm sách vào giỏ hàng.";
                return RedirectToAction("Details", "Home", new { id = bookId });
            }
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemRequest request)
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
                }

                var cartItem = await _cartService.UpdateCartItemAsync(request);

                return Json(new
                {
                    success = true,
                    message = "Đã cập nhật số lượng!",
                    cartItem = cartItem
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item quantity");
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi cập nhật số lượng."
                });
            }
        }

        // POST: /Cart/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
                }

                var success = await _cartService.RemoveFromCartAsync(cartItemId);

                if (success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Đã xóa sách khỏi giỏ hàng!"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Không thể xóa sách khỏi giỏ hàng."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi xóa sách."
                });
            }
        }

        // POST: /Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                if (!IsAuthenticated())
                {
                    TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Auth");
                }

                var userId = GetCurrentUserId().Value;
                var success = await _cartService.ClearCartAsync(userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Đã xóa tất cả sách trong giỏ hàng!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa giỏ hàng.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa giỏ hàng.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Cart/GetCartCount (for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return Json(new { count = 0 });
                }

                var userId = GetCurrentUserId().Value;
                var count = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(new { count = 0 });
            }
        }

        // GET: /Cart/CheckBookInCart (for AJAX)
        [HttpGet]
        public async Task<IActionResult> CheckBookInCart(int bookId)
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return Json(new { exists = false });
                }

                var userId = GetCurrentUserId().Value;
                var exists = await _cartService.IsBookInCartAsync(userId, bookId);
                return Json(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking book in cart");
                return Json(new { exists = false });
            }
        }
    }
}