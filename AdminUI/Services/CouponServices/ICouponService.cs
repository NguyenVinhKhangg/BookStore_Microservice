using System.Collections.Generic;
using AdminUI.Models.Coupon;

namespace AdminUI.Services.CouponServices
{

        public interface ICouponService
        {
            Task<(IEnumerable<CouponViewModel> coupons, int totalCount)> GetAllAsync(
                string searchTerm, bool? statusFilter, decimal? discountFilter, int page, int pageSize);
            Task<CouponViewModel> GetByIdAsync(int id);
            Task CreateAsync(CouponViewModel coupon);
            Task UpdateAsync(int id, CouponViewModel coupon);
            Task DeactivateAsync(int id);
            Task ActivateAsync(int id);
        }
    }
