using AdminUI.Models.Book;
using AdminUI.Services.BookServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminUI.Controllers
{
    public class BookManagementController : Controller
    {
        private readonly IBookService _bookService;

        public BookManagementController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: Danh sách sách
        public async Task<IActionResult> Index(string searchTerm, int? categoryFilter, bool? isActiveFilter,
            string sortBy = "Title", string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            try
            {
                var (books, totalCount) = await _bookService.GetAllAsync(searchTerm, categoryFilter, isActiveFilter, sortBy, sortOrder, page, pageSize);

                var viewModel = books.Select(b => new BookViewModel
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Description = b.Description,
                    ISBN = b.ISBN,
                    Price = b.Price,
                    Discount = b.Discount,
                    Stock = b.Stock,
                    CategoryID = b.CategoryID,
                    AuthorName = b.AuthorName,
                    PublisherName = b.PublisherName,
                    ImageUrl = b.ImageUrl,
                    IsActive = b.IsActive,
                    CreatedAt = b.CreatedAt
                }).ToList();

                ViewBag.Filter = new BookSearchFilterViewModel
                {
                    SearchTerm = searchTerm,
                    CategoryFilter = categoryFilter,
                    IsActiveFilter = isActiveFilter,
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };
                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();

                return View(viewModel);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Filter = new BookSearchFilterViewModel();
                ViewBag.TotalCount = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = 0;
                ViewBag.CategoryOptions = new List<SelectListItem>();
                return View(new List<BookViewModel>());
            }
        }

        // GET: Chi tiết sách (cho modal)
        public async Task<IActionResult> GetBookDetail(int id)
        {
            try
            {
                var book = await _bookService.GetByIdAsync(id);
                if (book == null)
                    return NotFound();

                var viewModel = new BookViewModel
                {
                    BookID = book.BookID,
                    Title = book.Title,
                    Description = book.Description,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Discount = book.Discount,
                    Stock = book.Stock,
                    CategoryID = book.CategoryID,
                    AuthorName = book.AuthorName,
                    PublisherName = book.PublisherName,
                    ImageUrl = book.ImageUrl,
                    IsActive = book.IsActive,
                    CreatedAt = book.CreatedAt
                };

                ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
                return PartialView("_BookDetailPartial", viewModel);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Tạo sách mới
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
            return View();
        }

        // POST: Tạo sách mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
                return View(model);
            }

            try
            {
                await _bookService.CreateAsync(model);
                TempData["SuccessMessage"] = "Tạo sách mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
                return View(model);
            }
        }

        // GET: Chỉnh sửa sách
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var book = await _bookService.GetByIdAsync(id);
                if (book == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sách.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new UpdateBookViewModel
                {
                    BookID = book.BookID,
                    Title = book.Title,
                    Description = book.Description,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Discount = book.Discount,
                    CategoryID = book.CategoryID,
                    AuthorName = book.AuthorName,
                    PublisherName = book.PublisherName,
                    ImageUrl = book.ImageUrl,
                    IsActive = book.IsActive
                };

                ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
                return View(viewModel);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Chỉnh sửa sách
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateBookViewModel model)
        {
            if (id != model.BookID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
                return View(model);
            }

            try
            {
                var result = await _bookService.UpdateAsync(id, model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Cập nhật sách thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại.");
                    ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
                    return View(model);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.CategoryOptions = await _bookService.GetCategoryOptionsAsync();
                return View(model);
            }
        }

        // POST: Ẩn sách
        [HttpPost]
        public async Task<IActionResult> Hide(int id)
        {
            try
            {
                var result = await _bookService.HideAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Ẩn sách thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ẩn sách thất bại!" });
                }
            }
            catch (HttpRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Hiển thị lại sách
        [HttpPost]
        public async Task<IActionResult> Unhide(int id)
        {
            try
            {
                var result = await _bookService.UnhideAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Hiển thị lại sách thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Hiển thị lại sách thất bại!" });
                }
            }
            catch (HttpRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}