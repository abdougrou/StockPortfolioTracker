using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockPortfolioTracker.Models;

namespace StockPortfolioTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockPortfolioTracker.Models.Transaction> Transaction { get; set; } = default!;
        public DbSet<StockPortfolioTracker.Models.Portfolio> Portfolio { get; set; } = default!;
        public DbSet<TransactionStatus> TransactionStatus { get; set; } = default!;
    }
}
