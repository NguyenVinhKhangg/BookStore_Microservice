using AutoMapper;
using ReviewsApi.DTOs;
using ReviewsApi.Models;
using ReviewsApi.Repositories;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewsApi.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<ReviewCreateDto> _createValidator;
        private readonly IValidator<ReviewUpdateDto> _updateValidator;

        public ReviewService(
            IReviewRepository repository,
            IMapper mapper,
            IValidator<ReviewCreateDto> createValidator,
            IValidator<ReviewUpdateDto> updateValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllAsync()
        {
            var reviews = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);
        }

        public async Task<ReviewReadDto> GetByIdAsync(int id)
        {
            var review = await _repository.GetByIdAsync(id);
            if (review == null || !review.IsActive)
                throw new KeyNotFoundException("Review not found or inactive");
            return _mapper.Map<ReviewReadDto>(review);
        }

        public async Task<ReviewReadDto> CreateAsync(ReviewCreateDto reviewDto)
        {
            var validationResult = await _createValidator.ValidateAsync(reviewDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var review = _mapper.Map<Review>(reviewDto);
            review.ReviewDate = DateTime.UtcNow;
            review.IsActive = true;
            await _repository.AddAsync(review);
            return _mapper.Map<ReviewReadDto>(review);
        }

        public async Task UpdateAsync(int id, ReviewUpdateDto reviewDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(reviewDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var review = await _repository.GetByIdAsync(id);
            if (review == null || !review.IsActive)
                throw new KeyNotFoundException("Review not found or inactive");

            _mapper.Map(reviewDto, review);
            await _repository.UpdateAsync(review);
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _repository.GetByIdAsync(id);
            if (review == null || !review.IsActive)
                throw new KeyNotFoundException("Review not found or already inactive");

            review.IsActive = false;
            await _repository.UpdateAsync(review);
        }
    }
}