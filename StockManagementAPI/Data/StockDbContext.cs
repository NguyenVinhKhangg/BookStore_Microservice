using Microsoft.EntityFrameworkCore;
using StockManagementApi.Models;

namespace StockManagementApi.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {
        }
        
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<StockTransactionDetail> StockTransactionDetails { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Unique constraint on TransactionID and BookID
            modelBuilder.Entity<StockTransactionDetail>()
                .HasIndex(d => new { d.TransactionID, d.BookID })
                .IsUnique();
                
            // FK relationship
            modelBuilder.Entity<StockTransactionDetail>()
                .HasOne(d => d.Transaction)
                .WithMany(t => t.Details)
                .HasForeignKey(d => d.TransactionID)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Transaction type check constraint
            modelBuilder.Entity<StockTransaction>()
                .HasCheckConstraint("CK_StockTransactions_TransactionType", 
                    "TransactionType IN ('StockIn', 'StockOut', 'Adjustment')");
                
            // Status check constraint
            modelBuilder.Entity<StockTransaction>()
                .HasCheckConstraint("CK_StockTransactions_Status", 
                    "Status IN ('Pending', 'Approved', 'Rejected')");
        }
    }
}