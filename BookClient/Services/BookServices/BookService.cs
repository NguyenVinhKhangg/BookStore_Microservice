using BookClient.Models;
using BookClient.Models.Book;
using BookClient.Services.BookServices;
using System.Text.Json;

namespace BookClient.Services.BookServices
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _bookApiUrl;

        public BookService(HttpClient httpClient, ILogger<BookService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;

            var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "https://localhost:7000";
            _bookApiUrl = $"{gatewayBaseUrl}/gateway/books";

            _logger.LogInformation($"🔧 BookService initialized with Gateway: {gatewayBaseUrl}");
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                _logger.LogInformation($"📚 Getting all books from: {_bookApiUrl}");

                var response = await _httpClient.GetAsync(_bookApiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📡 Book API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var books = await _httpClient.GetFromJsonAsync<List<Book>>(_bookApiUrl, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Book>();

                    _logger.LogInformation($"✅ Retrieved {books.Count} books successfully");
                    return books;
                }
                else
                {
                    _logger.LogError($"❌ Book API failed: {response.StatusCode} - {responseContent}");
                    return new List<Book>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception getting books");
                return new List<Book>();
            }
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"📖 Getting book by ID: {id}");

                var url = $"{_bookApiUrl}/{id}/detailBook";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var book = await _httpClient.GetFromJsonAsync<Book>(url, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation($"✅ Retrieved book: {book?.Title ?? "null"}");
                    return book;
                }
                else
                {
                    _logger.LogError($"❌ Book API failed: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Exception getting book {id}");
                return null;
            }
        }

        public async Task<List<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            try
            {
                var allBooks = await GetAllBooksAsync();
                var filteredBooks = allBooks.Where(b => b.CategoryID == categoryId && b.IsActive).ToList();

                _logger.LogInformation($"✅ Retrieved {filteredBooks.Count} books for category {categoryId}");
                return filteredBooks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Exception getting books by category {categoryId}");
                return new List<Book>();
            }
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation($"🔍 Searching books with term: {searchTerm}");

                var allBooks = await GetAllBooksAsync();
                var searchResults = allBooks.Where(b =>
                    b.IsActive && (
                        b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        b.AuthorName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        b.ISBN.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (b.Description != null && b.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    )).ToList();

                _logger.LogInformation($"✅ Found {searchResults.Count} books matching '{searchTerm}'");
                return searchResults;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Exception searching books with term: {searchTerm}");
                return new List<Book>();
            }
        }

        // ✅ THÊM: Advanced filtering method
        public async Task<(List<Book> Books, int TotalCount)> GetBooksWithFilterAsync(SearchFilterViewModel filter)
        {
            try
            {
                _logger.LogInformation($"🔍 Getting books with filter: SearchTerm={filter.SearchTerm}, MinPrice={filter.MinPrice}, MaxPrice={filter.MaxPrice}");

                var allBooks = await GetAllBooksAsync();

                // Apply filters
                var filteredBooks = allBooks.Where(b => b.IsActive);

                // Search term filter
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    filteredBooks = filteredBooks.Where(b =>
                        b.Title.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        b.AuthorName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        b.ISBN.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (b.Description != null && b.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    );
                }

                // Category filter
                if (!string.IsNullOrWhiteSpace(filter.CategoryIds))
                {
                    var categoryIds = filter.CategoryIds.Split(',')
                        .Where(id => int.TryParse(id, out _))
                        .Select(int.Parse)
                        .ToList();

                    if (categoryIds.Any())
                    {
                        filteredBooks = filteredBooks.Where(b => categoryIds.Contains(b.CategoryID));
                    }
                }

                // Price filter
                if (filter.MinPrice.HasValue)
                {
                    filteredBooks = filteredBooks.Where(b => GetFinalPrice(b) >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    filteredBooks = filteredBooks.Where(b => GetFinalPrice(b) <= filter.MaxPrice.Value);
                }

                // Stock filter
                if (!filter.IncludeOutOfStock)
                {
                    filteredBooks = filteredBooks.Where(b => b.Stock > 0);
                }

                var totalCount = filteredBooks.Count();

                // Sorting
                filteredBooks = filter.SortBy?.ToLower() switch
                {
                    "price" => filter.SortOrder?.ToLower() == "desc"
                        ? filteredBooks.OrderByDescending(b => GetFinalPrice(b))
                        : filteredBooks.OrderBy(b => GetFinalPrice(b)),
                    "author" => filter.SortOrder?.ToLower() == "desc"
                        ? filteredBooks.OrderByDescending(b => b.AuthorName)
                        : filteredBooks.OrderBy(b => b.AuthorName),
                    "stock" => filter.SortOrder?.ToLower() == "desc"
                        ? filteredBooks.OrderByDescending(b => b.Stock)
                        : filteredBooks.OrderBy(b => b.Stock),
                    _ => filter.SortOrder?.ToLower() == "desc"
                        ? filteredBooks.OrderByDescending(b => b.Title)
                        : filteredBooks.OrderBy(b => b.Title)
                };

                // Pagination
                var skip = (filter.Page - 1) * filter.PageSize;
                var pagedBooks = filteredBooks.Skip(skip).Take(filter.PageSize).ToList();

                _logger.LogInformation($"✅ Filtered {totalCount} books, returning {pagedBooks.Count} for page {filter.Page}");

                return (pagedBooks, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception filtering books");
                return (new List<Book>(), 0);
            }
        }

        // ✅ Helper method để tính giá sau discount
        private decimal GetFinalPrice(Book book)
        {
            if (book.Discount > 0)
            {
                return book.Price * (1 - book.Discount / 100);
            }
            return book.Price;
        }
    }
}