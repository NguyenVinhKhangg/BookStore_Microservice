using CouponsApi.Data;
using CouponsApi.Repositories;
using CouponsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CouponsApi.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly CouponDbContext _context;

        public CouponRepository(CouponDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coupons>> GetAllAsync()
        {
            return await _context.Coupon
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Coupons> GetByIdAsync(int id)
        {
            return await _context.Coupon
                .FirstOrDefaultAsync(c => c.CouponID == id);
        }

        public async Task<Coupons> GetByCodeAsync(string code)
        {
            return await _context.Coupon
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task AddAsync(Coupons coupon)
        {
            _context.Coupon.Add(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupons coupon)
        {
            _context.Entry(coupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var coupon = await _context.Coupon.FindAsync(id);
            if (coupon != null) {
                _context.Coupon.Remove(coupon);
                await _context.SaveChangesAsync();
            }
        }
    }
}