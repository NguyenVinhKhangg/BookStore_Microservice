namespace CartManagementApi.DTOs
{
    public class CartSummaryDto
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsEmpty => TotalItems == 0;
    }
}