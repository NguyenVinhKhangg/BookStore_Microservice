public interface IBooksImgRepository
{
    Task<BooksImg> AddBookImgAsync(BooksImg img);
    Task<bool> UpdateBookImgAsync(int imageId, BooksImgUpdateDto dto);
    Task<IEnumerable<BooksImg>> GetBookImgsByBookIdAsync(int bookId);
    Task<BooksImg?> GetBookImgByIdAsync(int imageId);
}
