using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPortfolioTracker.Models
{
    public class Portfolio
    {
        [Key] 
        public int PortfolioID { get; set; }

        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        public decimal TotalValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // FK
        public ICollection<Transaction> Transactions { get; set; }
    }
}
