using System.Net.Http.Json;
using System.Net;
using AdminUI.Models.Coupon;
using AdminUI.Services.CouponServices;

namespace AdminUI.Services.CouponServices
{
    public class CouponService : ICouponService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CouponService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri("http://localhost:5000/"); // Replace with actual API Gateway URL
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token)) {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<(IEnumerable<CouponViewModel> coupons, int totalCount)> GetAllAsync(
    string searchTerm, bool? statusFilter, decimal? discountFilter, int page, int pageSize)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/coupons");
            if (response.StatusCode == HttpStatusCode.NotFound) {
                return (Enumerable.Empty<CouponViewModel>(), 0);
            }

            response.EnsureSuccessStatusCode();
            var coupons = await response.Content.ReadFromJsonAsync<IEnumerable<CouponViewModel>>();

            // Client-side filtering
            var filteredCoupons = coupons.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm)) {
                filteredCoupons = filteredCoupons.Where(c => c.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }
            if (statusFilter.HasValue) {
                filteredCoupons = filteredCoupons.Where(c => c.IsActive == statusFilter.Value);
            }
            if (discountFilter.HasValue) {
                filteredCoupons = filteredCoupons.Where(c => Math.Abs(c.DiscountPercent - discountFilter.Value) < 0.01m);
            }

            // Client-side pagination
            var totalCount = filteredCoupons.Count();
            var pagedCoupons = filteredCoupons
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedCoupons, totalCount);
        }

        public async Task<CouponViewModel> GetByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/coupons/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) {
                throw new HttpRequestException("Mã giảm giá không tồn tại hoặc không hoạt động.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CouponViewModel>();
        }

        public async Task CreateAsync(CouponViewModel coupon)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/coupons", coupon);
            if (response.StatusCode == HttpStatusCode.BadRequest) {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Lỗi khi tạo mã giảm giá: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, CouponViewModel coupon)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/coupons/{id}", coupon);
            if (response.StatusCode == HttpStatusCode.BadRequest) {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Lỗi khi cập nhật mã giảm giá: {errorContent}");
            }
            if (response.StatusCode == HttpStatusCode.NotFound) {
                throw new HttpRequestException("Mã giảm giá không tồn tại hoặc không hoạt động.");
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task DeactivateAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/coupons/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) {
                throw new HttpRequestException("Mã giảm giá không tồn tại hoặc đã bị hủy kích hoạt.");
            }
            if (response.StatusCode == HttpStatusCode.BadRequest) {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Lỗi khi hủy kích hoạt mã giảm giá: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task ActivateAsync(int id)
        {
            var coupon = await GetByIdAsync(id);
            coupon.IsActive = true;
            await UpdateAsync(id, coupon);
        }
}

}
