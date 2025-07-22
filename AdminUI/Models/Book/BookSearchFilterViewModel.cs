namespace AdminUI.Models.Book
{
    public class BookSearchFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public int? CategoryFilter { get; set; }
        public bool? IsActiveFilter { get; set; }
        public string? SortBy { get; set; } = "Title";
        public string? SortOrder { get; set; } = "asc";
    }
}
