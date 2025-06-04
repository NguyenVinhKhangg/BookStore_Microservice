using CouponsApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CouponsApi.Services
{

    public interface ICouponService
    {
        Task<IEnumerable<CouponReadDto>> GetAllAsync();
        Task<CouponReadDto> GetByIdAsync(int id);
        Task<CouponReadDto> CreateAsync(CouponCreateDto couponDto);
        Task UpdateAsync(int id, CouponUpdateDto couponDto);
        Task DeleteAsync(int id);
    }
}