namespace BookClient.Models.Cart
{
    public class CartSummaryViewModel
    {
        public List<CartViewModel> Items { get; set; } = new List<CartViewModel>();
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal Subtotal => Items.Sum(i => i.Total);
        public decimal Tax => Subtotal * 0.1m; // 10% VAT
        public decimal Total => Subtotal + Tax;
        public int UniqueItems => Items.Count;
    }
}
