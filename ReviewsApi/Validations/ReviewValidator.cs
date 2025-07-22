using FluentValidation;
using ReviewsApi.DTOs;
using ReviewsApi.Models;

namespace ReviewsApi.Validations
{
    public class ReviewValidator : AbstractValidator<object>
    {
        public ReviewValidator()
        {
            // Validation for ReviewCreateDto
            When(dto => dto is ReviewCreateDto, () =>
            {
                RuleFor(dto => ((ReviewCreateDto)dto).BookID)
                    .GreaterThan(0).WithMessage("BookID must be greater than 0.");

                RuleFor(dto => ((ReviewCreateDto)dto).UserID)
                    .GreaterThan(0).WithMessage("UserID must be greater than 0.");

                RuleFor(dto => ((ReviewCreateDto)dto).Rating)
                    .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

                RuleFor(dto => ((ReviewCreateDto)dto).Comment)
                    .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.")
                    .When(dto => ((ReviewCreateDto)dto).Comment != null);
            });

            // Validation for ReviewUpdateDto
            When(dto => dto is ReviewUpdateDto, () =>
            {
                RuleFor(dto => ((ReviewUpdateDto)dto).ReviewID)
                    .GreaterThan(0).WithMessage("ReviewID must be greater than 0.");

                RuleFor(dto => ((ReviewUpdateDto)dto).BookID)
                    .GreaterThan(0).WithMessage("BookID must be greater than 0.");

                RuleFor(dto => ((ReviewUpdateDto)dto).UserID)
                    .GreaterThan(0).WithMessage("UserID must be greater than 0.");

                RuleFor(dto => ((ReviewUpdateDto)dto).Rating)
                    .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

                RuleFor(dto => ((ReviewUpdateDto)dto).Comment)
                    .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.")
                    .When(dto => ((ReviewUpdateDto)dto).Comment != null);
            });
        }
    }
}
