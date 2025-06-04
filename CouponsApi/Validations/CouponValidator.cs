using CouponsApi.DTOs;
using CouponsApi.Repositories;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace CouponsApi.Validations
{
    public class CouponValidator : AbstractValidator<object>
    {
        private readonly ICouponRepository _repository;

        public CouponValidator(ICouponRepository repository)
        {
            _repository = repository;

            // Validation for CouponCreateDto
            When(dto => dto is CouponCreateDto, () =>
            {
                RuleFor(dto => ((CouponCreateDto)dto).Code)
                    .NotEmpty().WithMessage("Code is required.")
                    .Length(1, 50).WithMessage("Code must be between 1 and 50 characters.")
                    .MustAsync(BeUniqueCode).WithMessage("Coupon code already exists.");

                RuleFor(dto => ((CouponCreateDto)dto).DiscountPercent)
                    .InclusiveBetween(0.01m, 99.99m).WithMessage("DiscountPercent must be between 0.01 and 99.99.");

                RuleFor(dto => ((CouponCreateDto)dto).ValidFrom)
                    .NotEmpty().WithMessage("ValidFrom is required.");

                RuleFor(dto => ((CouponCreateDto)dto).ValidTo)
                    .NotEmpty().WithMessage("ValidTo is required.")
                    .GreaterThan(dto => ((CouponCreateDto)dto).ValidFrom).WithMessage("ValidTo must be after ValidFrom.");
            });

            // Validation for CouponUpdateDto
            When(dto => dto is CouponUpdateDto, () =>
            {
                RuleFor(dto => ((CouponUpdateDto)dto).CouponID)
                    .GreaterThan(0).WithMessage("CouponID must be greater than 0.");

                RuleFor(dto => ((CouponUpdateDto)dto).Code)
                    .NotEmpty().WithMessage("Code is required.")
                    .Length(1, 50).WithMessage("Code must be between 1 and 50 characters.")
                    .MustAsync((dto, code, cancellation) => BeUniqueCodeForUpdate(code, ((CouponUpdateDto)dto).CouponID, cancellation))
                    .WithMessage("Coupon code already exists.");

                RuleFor(dto => ((CouponUpdateDto)dto).DiscountPercent)
                    .InclusiveBetween(0.01m, 99.99m).WithMessage("DiscountPercent must be between 0.01 and 99.99.");

                RuleFor(dto => ((CouponUpdateDto)dto).ValidFrom)
                    .NotEmpty().WithMessage("ValidFrom is required.");

                RuleFor(dto => ((CouponUpdateDto)dto).ValidTo)
                    .NotEmpty().WithMessage("ValidTo is required.")
                    .GreaterThan(dto => ((CouponUpdateDto)dto).ValidFrom).WithMessage("ValidTo must be after ValidFrom.");
            });
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellation)
        {
            var existingCoupon = await _repository.GetByCodeAsync(code);
            return existingCoupon == null;
        }

        private async Task<bool> BeUniqueCodeForUpdate(string code, int couponId, CancellationToken cancellation)
        {
            var existingCoupon = await _repository.GetByCodeAsync(code);
            return existingCoupon == null || existingCoupon.CouponID == couponId;
        }
    }
}
