using AdminUI.Models.Stock;

namespace AdminUI.Services
{
    public interface IStockService
    {
        Task<StockTransactionsListResponseModel> GetTransactionsAsync(StockSearchFilterViewModel filter);
        Task<StockTransactionResponseModel> GetTransactionByIdAsync(int id);
        Task<StockTransactionResponseModel> CreateTransactionAsync(CreateStockTransactionViewModel model);
        Task<StockTransactionResponseModel> UpdateTransactionStatusAsync(int id, UpdateTransactionStatusViewModel model);
        Task<StockTransactionResponseModel> DeleteTransactionAsync(int id);
    }
}