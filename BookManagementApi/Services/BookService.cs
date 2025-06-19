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


        public BookService(
            IBookRepository repo,
            IMapper mapper,
            ICategoryApiClient cateApi)
        {
            _repo = repo;
            _mapper = mapper;
            _cateApi = cateApi;
        }

        public async Task<BookDto> AddBookAsync(BookCreateDto dto)
        {
            // 1. Kiểm tra Category tồn tại
            var cate = await _cateApi.GetCategoryByIdAsync(dto.CategoryID);
            if (cate == null)
                throw new ArgumentException($"Category ID {dto.CategoryID} không hợp lệ.");

            // 2. Mapping và gán FK
            var book = _mapper.Map<Book>(dto);
            book.CreatedAt = DateTime.UtcNow;
            book.IsActive = true;
            book.CategoryID = dto.CategoryID;

            // 3. Lưu sách
            var added = await _repo.AddAsync(book);

            return _mapper.Map<BookDto>(added);
        }

        public async Task<IEnumerable<BookDto>> GetBooksAsync()
        {
            var books = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
        public async Task<BookDto> GetBookDetailAsync(int id)
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
            try
            {
                var book = await _repo.GetByIdAsync(bookId);
                if (book == null) return false;

                // Cập nhật số lượng
                int newStock = book.Stock + quantityChange;
                
                // Đảm bảo số lượng không âm
                if (newStock < 0) 
                {
                    newStock = 0;
                    // Log cảnh báo
                    // _logger.LogWarning($"Book {bookId} stock would be negative. Setting to 0.");
                }
                
                book.Stock = newStock;
                
                // Nếu là nhập kho, có thể cập nhật giá
                // if (quantityChange > 0) { ... }
                
                return await _repo.UpdateAsync(book);
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }
    }
}
