public class BooksImgDto
{
    public int ImageID { get; set; }
    public int BookID { get; set; }
    public string ImageUrl { get; set; }
    public string? Caption { get; set; }
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
    public DateTime UploadedAt { get; set; }
}
