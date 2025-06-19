using Microsoft.AspNetCore.Mvc;
using BookManagementApi.Services;
using BookManagementApi.DTOs;

namespace BookManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // Add new book
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDto dto)
        {
            var created = await _bookService.AddBookAsync(dto);
            if (created == null)
            {
                return BadRequest(new { message = "Failed to add the book." });
            }
            // Return 201 Created + info
            return CreatedAtAction(nameof(GetBook), new { id = created.BookID }, new { message = "Book has been added successfully.", data = created });
        }

        // Get all books
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _bookService.GetBooksAsync();
            return Ok(books);
        }

        // Get book detail by id
        [HttpGet("{id}/detailBook")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookService.GetBookDetailAsync(id);
            if (book == null)
                return NotFound(new { message = "Book not found." });
            return Ok(book);
        }

        // Update book
        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDto dto)
        {
            var result = await _bookService.UpdateBookAsync(id, dto);
            if (!result)
                return NotFound(new { message = "Book not found. Update failed." });

            return Ok(new { message = "Book has been updated successfully." });
        }

        // Soft delete (hide) book
        [HttpDelete("{id}/hideBook")]
        public async Task<IActionResult> HideBook(int id)
        {
            var existed = await _bookService.BookExistsAsync(id);
            if (!existed)
                return NotFound(new { message = "Book not found." });

            await _bookService.HideBookAsync(id);
            return Ok(new { message = "Book has been hidden successfully." });
        }

        // Unhide book
        [HttpPut("{id}/unhide")]
        public async Task<IActionResult> UnhideBook(int id)
        {
            var existed = await _bookService.BookExistsAsync(id);
            if (!existed)
                return NotFound(new { message = "Book not found." });

            var result = await _bookService.UnhideBookAsync(id);
            if (!result)
                return BadRequest(new { message = "Failed to unhide the book." });

            return Ok(new { message = "Book has been restored successfully." });
        }
    }
}






