public interface IBooksImgService
{
    Task<BooksImgDto> AddBookImgAsync(BooksImgCreateDto dto);
    Task<bool> UpdateBookImgAsync(int imageId, BooksImgUpdateDto dto);
    Task<IEnumerable<BooksImgDto>> GetBookImgsByBookIdAsync(int bookId);
    Task<BooksImgDto?> GetBookImgByIdAsync(int imageId);
}
