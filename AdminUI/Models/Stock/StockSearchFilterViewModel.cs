namespace AdminUI.Models.Stock
{
    public class StockSearchFilterViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "TransactionDate";
        public string SortOrder { get; set; } = "desc";
    }
}