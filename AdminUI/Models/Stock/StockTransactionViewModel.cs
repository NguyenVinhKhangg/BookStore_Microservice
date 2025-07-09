using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Stock
{
    public class StockTransactionViewModel
    {
        public int TransactionID { get; set; }
        
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        
        [Display(Name = "Creator Name")]
        public string CreatorName { get; set; }
        
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }
        
        [Display(Name = "Status")]
        public string Status { get; set; }
        
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        
        [Display(Name = "Approved By")]
        public int? ApprovedBy { get; set; }
        
        [Display(Name = "Approver Name")]
        public string ApproverName { get; set; }
        
        [Display(Name = "Approved At")]
        public DateTime? ApprovedAt { get; set; }
        
        [Display(Name = "Note")]
        public string Note { get; set; }
        
        [Display(Name = "Total Items")]
        public int TotalItems { get; set; }
        
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        
        public List<StockTransactionDetailViewModel> Details { get; set; } = new();
    }
}