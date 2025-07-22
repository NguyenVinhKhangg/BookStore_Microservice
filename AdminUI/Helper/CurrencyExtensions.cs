namespace AdminUI.Extensions
{
    public static class CurrencyExtensions
    {
        public static string ToVND(this decimal amount)
        {
            return amount.ToString("N0") + " VND";
        }

        public static string ToVND(this int amount)
        {
            return amount.ToString("N0") + " VND";
        }
    }
}