using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockPortfolioTracker.Models
{
    public class Stock
    {
        [Key]
        public int StockID { get; set; }

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Current Price")]
        public decimal CurrentPrice { get; set; }

        [DisplayName("Last Update Date")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // FK
        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<StockPriceHistory> StockPriceHistories { get; set; }
    }
}
