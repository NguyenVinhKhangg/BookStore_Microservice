using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminUI.Services.ReviewServices
{
    public class ReviewService : IReviewService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReviewService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token)) {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<(List<ReviewReadDto> Reviews, int TotalCount)> GetAllAsync(string? searchTerm, bool? statusFilter, int? bookFilter, int pageNumber, int pageSize)
        {
            AddAuthorizationHeader();
            var query = $"api/reviews?$skip={(pageNumber - 1) * pageSize}&$top={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm))
                query += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (statusFilter.HasValue)
                query += $"&isActive={statusFilter.Value}";
            if (bookFilter.HasValue)
                query += $"&bookId={bookFilter.Value}";

            var response = await _httpClient.GetAsync(query);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return (new List<ReviewReadDto>(), 0);
            response.EnsureSuccessStatusCode();
            var reviews = await response.Content.ReadFromJsonAsync<List<ReviewReadDto>>();
            var totalCount = int.Parse(response.Headers.GetValues("X-Total-Count").FirstOrDefault() ?? "0");
            return (reviews, totalCount);
        }

        public async Task<ReviewReadDto> GetByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/reviews/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException("Đánh giá không tồn tại");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ReviewReadDto>();
        }

        public async Task<ReviewReadDto> CreateAsync(ReviewCreateDto reviewDto)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/reviews", reviewDto);
            if (response.StatusCode == HttpStatusCode.BadRequest) {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Lỗi khi tạo đánh giá: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ReviewReadDto>();
        }

        public async Task UpdateAsync(int id, ReviewUpdateDto reviewDto)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/reviews/{id}", reviewDto);
            if (response.StatusCode == HttpStatusCode.BadRequest) {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Lỗi khi cập nhật đánh giá: {errorContent}");
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException("Đánh giá không tồn tại");
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/reviews/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new HttpRequestException("Đánh giá không tồn tại");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<SelectListItem>> GetBookOptionsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/books");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<SelectListItem>();
            response.EnsureSuccessStatusCode();
            var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
            return books.Select(b => new SelectListItem
            {
                Value = b.BookID.ToString(),
                Text = b.Title
            }).ToList();
        }

        public async Task<List<SelectListItem>> GetUserOptionsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/users");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<SelectListItem>();
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            return users.Select(u => new SelectListItem
            {
                Value = u.UserID.ToString(),
                Text = u.Username
            }).ToList();
        }
    }

    public class ReviewReadDto
    {
        public int ReviewID { get; set; }
        public int BookID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string BookTitle { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class ReviewCreateDto
    {
        public int BookID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ReviewUpdateDto
    {
        public int ReviewID { get; set; }
        public int BookID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
    }

    public class BookDto
    {
        public int BookID { get; set; }
        public string Title { get; set; }
    }

    public class UserDto
    {
        public int UserID { get; set; }
        public string Username { get; set; }
    }
}
