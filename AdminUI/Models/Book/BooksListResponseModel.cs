namespace AdminUI.Models.Book
{
    public class BooksListResponseModel
    {
        public List<BookResponseModel> Books { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
