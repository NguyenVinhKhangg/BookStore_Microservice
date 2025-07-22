using StockManagementAPI.DTOs;

namespace StockManagementApi.Services.Interfaces
{
    public interface IStockService
    {
        Task<StockTransactionDTO> GetTransactionByIdAsync(int id);
        Task<StockTransactionDTO> CreateTransactionAsync(CreateStockTransactionDTO createDto, int currentUserId);
        Task<StockTransactionDTO> UpdateTransactionStatusAsync(int id, UpdateTransactionStatusDTO updateDto, int currentUserId);
        Task<bool> DeleteTransactionAsync(int id);
        IQueryable<StockTransactionDTO> GetTransactionsQueryable();
        Task<int> GetTransactionCountAsync(string? searchTerm = null, string? transactionType = null,
            string? status = null, int? createdBy = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}