using BookManagementApi.Data;
using Microsoft.EntityFrameworkCore;

public class BooksImgRepository : IBooksImgRepository
{
    private readonly AppDbContext _context;
    public BooksImgRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BooksImg> AddBookImgAsync(BooksImg img)
    {
        _context.BooksImgs.Add(img);
        await _context.SaveChangesAsync();
        return img;
    }

    public async Task<bool> UpdateBookImgAsync(int imageId, BooksImgUpdateDto dto)
    {
        var img = await _context.BooksImgs.FindAsync(imageId);
        if (img == null) return false;

        if (dto.Caption != null) img.Caption = dto.Caption;
        if (dto.IsCover.HasValue) img.IsCover = dto.IsCover.Value;
        if (dto.SortOrder.HasValue) img.SortOrder = dto.SortOrder.Value;
        if (!string.IsNullOrEmpty(dto.ImageUrl)) img.ImageUrl = dto.ImageUrl;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BooksImg>> GetBookImgsByBookIdAsync(int bookId)
    {
        return await _context.BooksImgs
            .Where(x => x.BookID == bookId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
    }

    public async Task<BooksImg?> GetBookImgByIdAsync(int imageId)
    {
        return await _context.BooksImgs.FindAsync(imageId);
    }
}
