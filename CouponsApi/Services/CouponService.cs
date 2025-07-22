using AutoMapper;
using CouponsApi.DTOs;
using CouponsApi.Models;
using CouponsApi.Repositories;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CouponsApi.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<CouponDTO> _createValidator;
        private readonly IValidator<CouponUpdateDto> _updateValidator;

        public CouponService(
            ICouponRepository repository,
            IMapper mapper,
            IValidator<CouponDTO> createValidator,
            IValidator<CouponUpdateDto> updateValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<CouponReadDto>> GetAllAsync()
        {
            var coupons = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CouponReadDto>>(coupons);
        }

        public async Task<CouponReadDto> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Coupon ID must be greater than 0.");

            var coupon = await _repository.GetByIdAsync(id);
            if (coupon == null || !coupon.IsActive)
                throw new KeyNotFoundException("Coupon not found or inactive");

            return _mapper.Map<CouponReadDto>(coupon);
        }

        public async Task<CouponReadDto> GetByCodeAsync(string code)
        {
            var coupon = await _repository.GetByCodeAsync(code);
            if (coupon == null || !coupon.IsActive)
                throw new KeyNotFoundException("Coupon not found or inactive");

            return _mapper.Map<CouponReadDto>(coupon);
        }

        public async Task<CouponReadDto> CreateAsync(CouponDTO couponDto)
        {
            var validationResult = await _createValidator.ValidateAsync(couponDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var coupon = _mapper.Map<Coupons>(couponDto);
            await _repository.AddAsync(coupon);
            return _mapper.Map<CouponReadDto>(coupon);
        }

        public async Task UpdateAsync(int id, CouponUpdateDto couponDto)
        {
            if (id <= 0)
                throw new ValidationException("Coupon ID must be greater than 0.");

            var validationResult = await _updateValidator.ValidateAsync(couponDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var coupon = await _repository.GetByIdAsync(id);
            if (coupon == null || !coupon.IsActive)
                throw new KeyNotFoundException("Coupon not found or inactive");

            _mapper.Map(couponDto, coupon);
            await _repository.UpdateAsync(coupon);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Coupon ID must be greater than 0.");

            var coupon = await _repository.GetByIdAsync(id);
            if (coupon == null || !coupon.IsActive)
                throw new KeyNotFoundException("Coupon not found or already inactive");

            coupon.IsActive = false;
            await _repository.UpdateAsync(coupon);
        }
    }
}