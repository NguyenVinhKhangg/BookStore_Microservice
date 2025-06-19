using BookManagementApi.DTOs;
using FluentValidation;
using BookManagementApi.DTOs;


namespace BookManagementApi.Validations
{
    public class BookUpdateDtoValidator : AbstractValidator<BookUpdateDto>
    {
        public BookUpdateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ISBN).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryID).NotEmpty();
            RuleFor(x => x.AuthorID).NotEmpty();
            RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PublisherID).NotEmpty();
            RuleFor(x => x.PublisherName).NotEmpty().MaximumLength(100);
        }
    }
}
