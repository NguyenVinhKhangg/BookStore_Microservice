namespace AdminUI.Models.Book
{
    public class SimpleResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
