using StockManagementApi.Models;

namespace StockManagementApi.Repositories.Interfaces
{
    public interface IStockTransactionRepository
    {
        Task<StockTransaction> GetByIdAsync(int id);
        Task<StockTransaction> CreateAsync(StockTransaction transaction);
        Task<StockTransaction> UpdateStatusAsync(int id, string status, int approvedBy, string note);
        Task<bool> DeleteAsync(int id);
        IQueryable<StockTransaction> GetQueryable();
    }
}