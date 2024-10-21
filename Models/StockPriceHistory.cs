using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPortfolioTracker.Models
{
    public class StockPriceHistory
    {
        [Key]
        public int PriceHistoryID { get; set; }

        public int StockID { get; set; }

        [ForeignKey("StockID")]
        public Stock Stock { get; set; }

        public DateTime Date { get; set; }

        public decimal ClosingPrice { get; set; }
    }
}
