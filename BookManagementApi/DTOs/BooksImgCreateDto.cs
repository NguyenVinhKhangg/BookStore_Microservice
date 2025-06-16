public class BooksImgCreateDto
{
    public int BookID { get; set; }
    public string ImageUrl { get; set; }
    public string? Caption { get; set; }
    public bool IsCover { get; set; } = false;
    public int SortOrder { get; set; } = 0;
}
