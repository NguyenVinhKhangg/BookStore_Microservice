using AdminUI.Models.Review;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using AdminUI.Services.ReviewServices;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminUI.Controllers
{
  
    public class ReviewManagementController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewManagementController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: Danh sách đánh giá
        public async Task<IActionResult> Index(string searchTerm, bool? statusFilter, int? bookFilter, int page = 1, int pageSize = 10)
        {
            try {
                var (reviews, totalCount) = await _reviewService.GetAllAsync(searchTerm, statusFilter, bookFilter, page, pageSize);
                var viewModel = reviews.Select(r => new ReviewViewModel
                {
                    ReviewID = r.ReviewID,
                    BookID = r.BookID,
                    UserID = r.UserID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate,
                    IsActive = r.IsActive
                }).ToList();

                ViewBag.Filter = new ReviewSearchFilterViewModel
                {
                    SearchTerm = searchTerm,
                    StatusFilter = statusFilter,
                    BookFilter = bookFilter
                };
                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                ViewBag.BookOptions = await _reviewService.GetBookOptionsAsync();
                ViewBag.UserOptions = await _reviewService.GetUserOptionsAsync();

                return View(viewModel);
            }
            catch (HttpRequestException ex) {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Filter = new ReviewSearchFilterViewModel();
                ViewBag.TotalCount = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = 0;
                ViewBag.BookOptions = new List<SelectListItem>();
                ViewBag.UserOptions = new List<SelectListItem>();
                return View(new List<ReviewViewModel>());
            }
        }

        // GET: Chi tiết đánh giá (cho modal)
        public async Task<IActionResult> GetReviewDetail(int id)
        {
            try {
                var review = await _reviewService.GetByIdAsync(id);
                var viewModel = new ReviewViewModel
                {
                    ReviewID = review.ReviewID,
                    BookID = review.BookID,
                    UserID = review.UserID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ReviewDate = review.ReviewDate,
                    IsActive = review.IsActive
                };
                ViewBag.BookOptions = await _reviewService.GetBookOptionsAsync();
                ViewBag.UserOptions = await _reviewService.GetUserOptionsAsync();
                return PartialView("_ReviewDetail", viewModel);
            }
            catch (HttpRequestException ex) {
                return Content($"<div class='alert alert-danger'><i class='fas fa-exclamation-triangle'></i> {ex.Message}</div>");
            }
        }

        // GET: Sửa đánh giá
        public async Task<IActionResult> Edit(int id)
        {
            try {
                var review = await _reviewService.GetByIdAsync(id);
                var viewModel = new ReviewViewModel
                {
                    ReviewID = review.ReviewID,
                    BookID = review.BookID,
                    UserID = review.UserID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ReviewDate = review.ReviewDate,
                    IsActive = review.IsActive,
                    BookOptions = await _reviewService.GetBookOptionsAsync(),
                    UserOptions = await _reviewService.GetUserOptionsAsync()
                };
                return View(viewModel);
            }
            catch (HttpRequestException ex) {
                ModelState.AddModelError("", ex.Message);
                return NotFound();
            }
        }

        // POST: Sửa đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReviewViewModel viewModel)
        {
            if (ModelState.IsValid) {
                try {
                    var reviewDto = new ReviewUpdateDto
                    {
                        ReviewID = viewModel.ReviewID,
                        BookID = viewModel.BookID,
                        UserID = viewModel.UserID,
                        Rating = viewModel.Rating,
                        Comment = viewModel.Comment,
                        IsActive = viewModel.IsActive
                    };
                    await _reviewService.UpdateAsync(id, reviewDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex) {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            viewModel.BookOptions = await _reviewService.GetBookOptionsAsync();
            viewModel.UserOptions = await _reviewService.GetUserOptionsAsync();
            return View(viewModel);
        }

        // POST: Xóa/Ẩn đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try {
                var review = await _reviewService.GetByIdAsync(id);
                var reviewDto = new ReviewUpdateDto
                {
                    ReviewID = review.ReviewID,
                    BookID = review.BookID,
                    UserID = review.UserID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    IsActive = false
                };
                await _reviewService.UpdateAsync(id, reviewDto);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex) {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Kích hoạt đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try {
                var review = await _reviewService.GetByIdAsync(id);
                var reviewDto = new ReviewUpdateDto
                {
                    ReviewID = review.ReviewID,
                    BookID = review.BookID,
                    UserID = review.UserID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    IsActive = true
                };
                await _reviewService.UpdateAsync(id, reviewDto);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex) {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

