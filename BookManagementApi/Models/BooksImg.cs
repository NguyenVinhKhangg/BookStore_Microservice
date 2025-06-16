using System.ComponentModel.DataAnnotations;

public class BooksImg
{
    [Key] // BẮT BUỘC phải có cho Entity Framework nhận diện đây là khóa chính
    public int ImageID { get; set; }

    public int BookID { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsCover { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    // Nếu có navigation property:
    // public Book? Book { get; set; }
}
