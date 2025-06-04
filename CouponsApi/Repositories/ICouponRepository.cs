using CouponsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CouponsApi.Repositories
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupons>> GetAllAsync();
        Task<Coupons> GetByIdAsync(int id);
        Task<Coupons> GetByCodeAsync(string code);
        Task AddAsync(Coupons coupon);
        Task UpdateAsync(Coupons coupon);
        Task DeleteAsync(int id);
    }
}
