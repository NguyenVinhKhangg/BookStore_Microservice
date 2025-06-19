using BookImgManagementApi.DTOs;
using BookImgManagementApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace BookImgManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksImgsController : ControllerBase
    {
        private readonly IBooksImgService _service;

        public BooksImgsController(IBooksImgService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<BooksImgReadDto>>> GetBooksImgs()
        {
            var imgs = await _service.GetAllAsync();
            return Ok(imgs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BooksImgReadDto>> GetBooksImg(int id)
        {
            try {
                var img = await _service.GetByIdAsync(id);
                return Ok(img);
            }
            catch (KeyNotFoundException) {
                return NotFound("Image not found");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BooksImgReadDto>> PostBooksImg(BooksImgCreateDto dto)
        {
            var img = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetBooksImg), new { id = img.ImageID }, img);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooksImg(int id, BooksImgUpdateDto dto)
        {
            if (id != dto.ImageID)
                return BadRequest("ImageID mismatch");
            try {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException) {
                return NotFound("Image not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooksImg(int id)
        {
            try {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) {
                return NotFound("Image not found");
            }
        }
    }
}
