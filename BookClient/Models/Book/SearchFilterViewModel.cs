using System.ComponentModel.DataAnnotations;

namespace BookClient.Models.Book
{
    public class SearchFilterViewModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Category")]
        public string? CategoryIds { get; set; }

        [Display(Name = "Min Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu phải lớn hơn 0")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tối đa phải lớn hơn 0")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Sort By")]
        public string? SortBy { get; set; } = "title";

        [Display(Name = "Sort Order")]
        public string? SortOrder { get; set; } = "asc";

        [Display(Name = "Show Out of Stock")]
        public bool IncludeOutOfStock { get; set; } = true;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}