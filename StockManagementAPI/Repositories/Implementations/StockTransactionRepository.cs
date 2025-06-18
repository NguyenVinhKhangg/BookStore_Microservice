using Microsoft.EntityFrameworkCore;
using StockManagementApi.Data;
using StockManagementApi.Models;
using StockManagementApi.Repositories.Interfaces;

namespace StockManagementApi.Repositories.Implementations
{
    public class StockTransactionRepository : IStockTransactionRepository
    {
        private readonly StockDbContext _context;
        
        public StockTransactionRepository(StockDbContext context)
        {
            _context = context;
        }
        
        public async Task<StockTransaction> GetByIdAsync(int id)
        {
            return await _context.StockTransactions
                .Include(t => t.Details)
                .FirstOrDefaultAsync(t => t.TransactionID == id);
        }
        
        public async Task<StockTransaction> CreateAsync(StockTransaction transaction)
        {
            _context.StockTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
        
        public async Task<StockTransaction> UpdateStatusAsync(int id, string status, int approvedBy, string note)
        {
            var transaction = await _context.StockTransactions.FindAsync(id);
            
            if (transaction == null)
                return null;
                
            transaction.Status = status;
            transaction.ApprovedBy = approvedBy;
            transaction.ApprovedAt = DateTime.UtcNow;
            
            if (!string.IsNullOrEmpty(note))
                transaction.Note = note;
                
            await _context.SaveChangesAsync();
            return transaction;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.StockTransactions.FindAsync(id);
            
            if (transaction == null)
                return false;
                
            _context.StockTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public IQueryable<StockTransaction> GetQueryable()
        {
            return _context.StockTransactions
                .Include(t => t.Details)
                .AsQueryable();
        }
    }
}