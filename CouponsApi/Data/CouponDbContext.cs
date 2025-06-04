using CouponsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CouponsApi.Data
{
    public class CouponDbContext : DbContext
    {
        public CouponDbContext(DbContextOptions<CouponDbContext> options) : base(options)
        {
        }

        public DbSet<Coupons> Coupon { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupons>()
                .HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}
