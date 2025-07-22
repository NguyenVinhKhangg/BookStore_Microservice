using FluentValidation;
using ReviewsApi.DTOs;
using ReviewsApi.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsApi.Validations
{
    public class ReviewUpdateValidator : AbstractValidator<ReviewUpdateDto>
    {
        private readonly IReviewRepository _repository;

        public ReviewUpdateValidator(IReviewRepository repository)
        {
            _repository = repository;

            RuleFor(dto => dto.ReviewID)
                .GreaterThan(0).WithMessage("ReviewID must be greater than 0.");

            RuleFor(dto => dto.BookID)
                .GreaterThan(0).WithMessage("BookID must be greater than 0.");

            RuleFor(dto => dto.UserID)
                .GreaterThan(0).WithMessage("UserID must be greater than 0.");

            RuleFor(dto => dto.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(dto => dto.Comment)
                .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.")
                .When(dto => dto.Comment != null);

            // Optional: Ensure unique BookID/UserID pair (excluding current review)
            RuleFor(dto => new { dto.BookID, dto.UserID, dto.ReviewID })
                .MustAsync(BeUniqueReviewForUpdate).WithMessage("A review for this book by this user already exists.");
        }

        private async Task<bool> BeUniqueReviewForUpdate(ReviewUpdateDto dto, dynamic key, CancellationToken cancellation)
        {
            var existingReview = await _repository.GetByBookIdAndUserIdAsync(dto.BookID, dto.UserID);
            return existingReview == null || existingReview.ReviewID == dto.ReviewID || !existingReview.IsActive;
        }
    }
}