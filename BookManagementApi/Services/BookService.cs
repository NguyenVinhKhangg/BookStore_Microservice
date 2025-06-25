using AutoMapper;
using BookManagementApi.ApiClients;
using BookManagementApi.Data;
using BookManagementApi.DTOs;
using BookManagementApi.Models;
using BookManagementApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookManagementApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICategoryApiClient _cateApi;
        private readonly AppDbContext _dbContext;


        public BookService(
            IBookRepository repo,
            IMapper mapper,
            ICategoryApiClient cateApi, AppDbContext dbContext)
        {
            _repo = repo;
            _mapper = mapper;
            _cateApi = cateApi;
            _dbContext = dbContext;
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

        public async Task<PageList<Book>> GetBooksByTitleOrAuthorOrPrice(
        string searchQuery,
        int pageNumber = 1,
        int pageSize = 8,
        string sortBy = "Title",
        bool ascending = true)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                throw new ArgumentException("Search query cannot be empty", nameof(searchQuery));

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 8;

            decimal? searchPrice = decimal.TryParse(searchQuery, out var p) ? p : null;

            var query = _dbContext.Books
                .Include(b => b.CategoryID)
                .Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.AuthorName.Contains(searchQuery) ||
                    (searchPrice.HasValue &&
                     b.Price >= searchPrice.Value * 0.9m &&
                     b.Price <= searchPrice.Value * 1.1m))
                .AsQueryable();

            var totalCount = await query.CountAsync();
            if (totalCount == 0)
                return new PageList<Book>(new List<Book>(), 0, pageNumber, pageSize);

            query = sortBy.ToLower() switch
            {
                "title" => ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title),
                "author" => ascending ? query.OrderBy(b => b.AuthorName) : query.OrderByDescending(b => b.AuthorName),
                "price" => ascending ? query.OrderBy(b => b.Price) : query.OrderByDescending(b => b.Price),
                "stock" => ascending ? query.OrderBy(b => b.Stock) : query.OrderByDescending(b => b.Stock),
                _ => query.OrderBy(b => b.Title)
            };

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageList<Book>(items, totalCount, pageNumber, pageSize);
        }
    }
}
