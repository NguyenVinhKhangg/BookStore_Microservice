using BookClient.Models;
using BookClient.Models.Book;
using BookClient.Services.BookServices;
using BookClient.Services.CategoryServices;
using Microsoft.AspNetCore.Mvc;

namespace BookClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IBookService bookService,
            ICategoryService categoryService,
            ILogger<HomeController> logger)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(SearchFilterViewModel filter)
        {
            // Set default values if not provided
            filter ??= new SearchFilterViewModel();

            ViewBag.SearchFilter = filter;

            _logger.LogInformation($"📥 Index request - SearchTerm: {filter.SearchTerm}, CategoryIds: {filter.CategoryIds}, MinPrice: {filter.MinPrice}, MaxPrice: {filter.MaxPrice}");

            // ✅ Sử dụng advanced filtering
            var (filteredBooks, totalCount) = await _bookService.GetBooksWithFilterAsync(filter);
            var categories = await _categoryService.GetAllCategoriesAsync();

            // Map CategoryName cho Books
            var categoryDict = categories.ToDictionary(c => c.CategoryID, c => c.Name);
            foreach (var book in filteredBooks)
            {
                book.CategoryName = book.CategoryID != 0 && categoryDict.ContainsKey(book.CategoryID)
                    ? categoryDict[book.CategoryID]
                    : "Không xác định";
            }

            // For ViewBag data
            ViewBag.Categories = categories;
            ViewBag.AllBooks = filteredBooks; // For hot deals section
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            // Selected categories for UI
            var selectedCategoryIds = string.IsNullOrWhiteSpace(filter.CategoryIds)
                ? new List<int>()
                : filter.CategoryIds.Split(',')
                    .Where(id => int.TryParse(id, out _))
                    .Select(int.Parse)
                    .ToList();
            ViewBag.SelectedCategoryIds = selectedCategoryIds;

            return View(filteredBooks);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"📖 Getting book details for ID: {id}");

            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                _logger.LogWarning($"⚠️ Book {id} not found");
                return NotFound();
            }

            // Get Category information
            if (book.CategoryID != 0)
            {
                var category = await _categoryService.GetCategoryByIdAsync(book.CategoryID);
                book.CategoryName = category?.Name ?? "Không xác định";
            }
            else
            {
                book.CategoryName = "Không xác định";
            }

            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> GetRelatedBooks(int categoryId, int currentBookId, int count = 4)
        {
            try
            {
                _logger.LogInformation($"🔗 Getting related books: CategoryID={categoryId}, CurrentBookID={currentBookId}, Count={count}");

                var relatedBooks = await _bookService.GetBooksByCategoryAsync(categoryId);
                relatedBooks = relatedBooks.Where(b => b.BookID != currentBookId).Take(count).ToList();

                var categories = await _categoryService.GetAllCategoriesAsync();
                var categoryDict = categories.ToDictionary(c => c.CategoryID, c => c.Name);

                foreach (var book in relatedBooks)
                {
                    book.CategoryName = categoryDict.ContainsKey(book.CategoryID)
                        ? categoryDict[book.CategoryID]
                        : "Không xác định";
                }

                _logger.LogInformation($"✅ Found {relatedBooks.Count} related books");

                return PartialView("_RelatedBooksPartial", relatedBooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error getting related books");
                return PartialView("_RelatedBooksPartial", new List<Book>());
            }
        }

        // ✅ THÊM: AJAX endpoint for live search
        [HttpGet]
        public async Task<IActionResult> SearchBooks(string term, int? categoryId, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            try
            {
                var filter = new SearchFilterViewModel
                {
                    SearchTerm = term,
                    CategoryIds = categoryId?.ToString(),
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Page = page,
                    PageSize = 8
                };

                var (books, totalCount) = await _bookService.GetBooksWithFilterAsync(filter);
                var categories = await _categoryService.GetAllCategoriesAsync();

                var categoryDict = categories.ToDictionary(c => c.CategoryID, c => c.Name);
                foreach (var book in books)
                {
                    book.CategoryName = categoryDict.ContainsKey(book.CategoryID)
                        ? categoryDict[book.CategoryID]
                        : "Không xác định";
                }

                return Json(new
                {
                    success = true,
                    books = books.Select(b => new
                    {
                        bookID = b.BookID,
                        title = b.Title,
                        authorName = b.AuthorName,
                        price = b.Price,
                        discount = b.Discount,
                        finalPrice = b.Discount > 0 ? b.Price * (1 - b.Discount / 100) : b.Price,
                        stock = b.Stock,
                        imageUrl = b.ImageUrl,
                        categoryName = b.CategoryName
                    }),
                    totalCount = totalCount,
                    totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                    currentPage = page
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Error searching books");
                return Json(new { success = false, message = "Error occurred while searching" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchCategories(string term)
        {
            try
            {
                var categories = string.IsNullOrWhiteSpace(term)
                    ? await _categoryService.GetAllCategoriesAsync()
                    : await _categoryService.SearchCategoriesAsync(term);

                return Json(new
                {
                    success = true,
                    data = categories.Select(c => new
                    {
                        id = c.CategoryID,
                        name = c.Name,
                        description = c.Description
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Error searching categories with term: {term}");
                return Json(new { success = false, message = "Error occurred while searching categories" });
            }
        }
    }
}