using Microsoft.AspNetCore.Mvc;

// Đánh dấu đây là một API controller và route theo tên controller (BooksImg)
[ApiController]
[Route("api/[controller]")]
public class BooksImgController : ControllerBase
{
    private readonly IBooksImgService _service;

    public BooksImgController(IBooksImgService service)
    {
        _service = service;
    }

    // Thêm mới một ảnh sách
    // POST: /api/BooksImg
    [HttpPost]
    public async Task<IActionResult> AddBookImg([FromBody] BooksImgCreateDto dto)
    {
        var img = await _service.AddBookImgAsync(dto);
        // Trả về thông báo thành công kèm dữ liệu ảnh vừa thêm
        return Ok(new { message = "Book image has been added successfully.", data = img });
    }

    // Cập nhật thông tin một ảnh sách
    // PUT: /api/BooksImg/{imageId}
    [HttpPut("{imageId}/updateIMG")]
    public async Task<IActionResult> UpdateBookImg(int imageId, [FromBody] BooksImgUpdateDto dto)
    {
        var result = await _service.UpdateBookImgAsync(imageId, dto);
        if (!result) 
            return NotFound(new { message = "Book image not found." });
        return Ok(new { message = "Book image has been updated successfully." });
    }

    // Lấy danh sách ảnh của một cuốn sách theo BookId
    // GET: /api/BooksImg/byBook/{bookId}
    [HttpGet("byBook/{bookId}")]
    public async Task<IActionResult> GetBookImgsByBookId(int bookId)
    {
        var imgs = await _service.GetBookImgsByBookIdAsync(bookId);
        return Ok(imgs);
    }

    // Lấy chi tiết một ảnh theo ImageId
    // GET: /api/BooksImg/{imageId}
    [HttpGet("{imageId}")]
    public async Task<IActionResult> GetBookImgById(int imageId)
    {
        var img = await _service.GetBookImgByIdAsync(imageId);
        if (img == null) 
            return NotFound(new { message = "Book image not found." });
        return Ok(img);
    }
}
