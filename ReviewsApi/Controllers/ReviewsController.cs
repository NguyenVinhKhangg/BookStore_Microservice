using ReviewsApi.DTOs;
using ReviewsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Query;

namespace ReviewsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;
        public ReviewsController(IReviewService service) => _service = service;

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetReviews()
        {
            var reviews = await _service.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewReadDto>> GetReview(int id)
        {
            try {
                var review = await _service.GetByIdAsync(id);
                return Ok(review);
            }
            catch (KeyNotFoundException) {
                return NotFound("Review not found or inactive");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ReviewReadDto>> PostReview(ReviewCreateDto reviewDto)
        {
            try {
                var review = await _service.CreateAsync(reviewDto);
                return CreatedAtAction(nameof(GetReview), new { id = review.ReviewID }, review);
            }
            catch (FluentValidation.ValidationException ex) {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, ReviewUpdateDto reviewDto)
        {
            if (id != reviewDto.ReviewID)
                return BadRequest("ReviewID mismatch");
            try {
                await _service.UpdateAsync(id, reviewDto);
                return NoContent();
            }
            catch (FluentValidation.ValidationException ex) {
                return BadRequest(ex.Errors);
            }
            catch (KeyNotFoundException) {
                return NotFound("Review not found or inactive");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (FluentValidation.ValidationException ex) {
                return BadRequest(ex.Errors);
            }
            catch (KeyNotFoundException) {
                return NotFound("Review not found or already inactive");
            }
        }
    }
}