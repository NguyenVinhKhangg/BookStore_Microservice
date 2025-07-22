namespace BookClient.Models.Cart
{
    public class CartResponseModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public CartViewModel? Data { get; set; }
    }

    public class CartListResponseModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<CartViewModel>? Data { get; set; }
        public int TotalCount { get; set; }
    }

    public class CartActionResponseModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? CartID { get; set; }
        public int? TotalItems { get; set; }
    }
}