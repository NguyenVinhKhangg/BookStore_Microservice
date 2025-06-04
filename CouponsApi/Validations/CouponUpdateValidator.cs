using CouponsApi.DTOs;
using CouponsApi.Repositories;
using FluentValidation;


namespace CouponsApi.Validations
{
    public class CouponUpdateValidator : AbstractValidator<CouponUpdateDto>
    {
        private readonly ICouponRepository _repository;

        public CouponUpdateValidator(ICouponRepository repository)
        {
            _repository = repository;

            RuleFor(dto => dto.CouponID)
                .GreaterThan(0).WithMessage("CouponID must be greater than 0.");

            RuleFor(dto => dto.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Length(1, 50).WithMessage("Code must be between 1 and 50 characters.")
                .MustAsync(BeUniqueCodeForUpdate).WithMessage("Coupon code already exists.");

            RuleFor(dto => dto.DiscountPercent)
                .InclusiveBetween(0.01m, 99.99m).WithMessage("DiscountPercent must be between 0.01 and 99.99.");

            RuleFor(dto => dto.ValidFrom)
                .NotEmpty().WithMessage("ValidFrom is required.");

            RuleFor(dto => dto.ValidTo)
                .NotEmpty().WithMessage("ValidTo is required.")
                .GreaterThan(dto => dto.ValidFrom).WithMessage("ValidTo must be after ValidFrom.");
        }

        private async Task<bool> BeUniqueCodeForUpdate(CouponUpdateDto dto, string code, CancellationToken cancellation)
        {
            var existingCoupon = await _repository.GetByCodeAsync(code);
            return existingCoupon == null || existingCoupon.CouponID == dto.CouponID;
        }
    }
}
