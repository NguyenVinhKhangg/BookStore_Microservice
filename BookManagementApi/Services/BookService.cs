using AutoMapper;
using BookManagementApi.ApiClients;
using BookManagementApi.DTOs;
using BookManagementApi.Models;
using BookManagementApi.Repositories;

namespace BookManagementApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICategoryApiClient _cateApi;
        private readonly ILogger<BookService> _logger; // ✅ THÊM

        public BookService(
            IBookRepository repo,
            IMapper mapper,
            ICategoryApiClient cateApi,
            ILogger<BookService> logger) // ✅ THÊM
        {
            _repo = repo;
            _mapper = mapper;
            _cateApi = cateApi;
            _logger = logger; // ✅ THÊM
        }

        public async Task<BookDto> AddBookAsync(BookCreateDto dto)
        {
            // 1. Kiểm tra Category tồn tại
            var cate = await _cateApi.GetCategoryByIdAsync(dto.CategoryID);
            if (cate == null)
                throw new ArgumentException($"Category ID {dto.CategoryID} không hợp lệ.");

            // 2. Mapping và đảm bảo các giá trị mặc định
            var book = _mapper.Map<Book>(dto);
            book.CreatedAt = DateTime.UtcNow;
            book.IsActive = true;
            book.CategoryID = dto.CategoryID;

            // 3. QUAN TRỌNG: Đảm bảo Stock = 0 khi tạo mới
            book.Stock = 0;

            // 4. Lưu sách
            var added = await _repo.AddAsync(book);

            return _mapper.Map<BookDto>(added);
        }

        // ✅ Chỉ sách active cho public
        public async Task<IEnumerable<BookDto>> GetBooksAsync()
        {
            var books = await _repo.GetAllActiveAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        // ✅ Tất cả sách cho admin
        public async Task<IEnumerable<BookDto>> GetAllBooksForAdminAsync()
        {
            var books = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        // ✅ Chi tiết sách với phân quyền
        public async Task<BookDto> GetBookDetailAsync(int id, bool isAdmin = false)
        {
            var book = await _repo.GetByIdAsync(id);

            // Nếu không phải admin và sách bị ẩn thì trả null
            if (!isAdmin && book != null && !book.IsActive)
            {
                return null;
            }

            return _mapper.Map<BookDto>(book);
        }

        // ✅ Chi tiết sách cho admin (luôn hiển thị)
        public async Task<BookDto> GetBookDetailForAdminAsync(int id)
        {
            var book = await _repo.GetByIdAsync(id);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<bool> UpdateBookAsync(int bookId, BookUpdateDto dto)
        {
            var book = await _repo.GetByIdAsync(bookId);
            if (book == null) return false;
            _mapper.Map(dto, book);
            return await _repo.UpdateAsync(book);
        }

        public async Task<bool> HideBookAsync(int id)
        {
            return await _repo.HideAsync(id);
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            var book = await _repo.GetByIdAsync(id);
            return book != null;
        }

        public async Task<bool> UnhideBookAsync(int id)
        {
            return await _repo.UnhideAsync(id);
        }

        public async Task<bool> UpdateBookStockAsync(int bookId, int quantityChange)
        {
            _logger.LogInformation($"🔥 UpdateBookStockAsync called - BookID: {bookId}, QuantityChange: {quantityChange}");

            try
            {
                var book = await _repo.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning($"❌ Book with ID {bookId} not found");
                    return false;
                }

                _logger.LogInformation($"📚 Current book stock: {book.Stock}");

                // Cập nhật số lượng
                int newStock = book.Stock + quantityChange;

                // Đảm bảo số lượng không âm
                if (newStock < 0)
                {
                    _logger.LogWarning($"⚠️ New stock would be negative ({newStock}), setting to 0");
                    newStock = 0;
                }

                book.Stock = newStock;
                _logger.LogInformation($"📦 Updating stock from {book.Stock - quantityChange} to {newStock}");

                var result = await _repo.UpdateAsync(book);

                if (result)
                {
                    _logger.LogInformation($"✅ Book {bookId} stock updated successfully to {newStock}");
                }
                else
                {
                    _logger.LogError($"❌ Failed to update book {bookId} stock in database");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Error updating book stock for BookID: {bookId}");
                return false;
            }
        }
    }
}