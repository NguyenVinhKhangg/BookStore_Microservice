namespace AdminUI.Models.Category
{
    public class CategoriesListResponseModel
    {
        public bool Success { get; set; }
        public IEnumerable<CategoryViewModel> Data { get; set; } = new List<CategoryViewModel>();
        public int TotalCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
