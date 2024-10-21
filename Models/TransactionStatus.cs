using System.ComponentModel.DataAnnotations;

namespace StockPortfolioTracker.Models
{
    public class TransactionStatus
    {
        [Key]
        public int TransactionStatusID { get; set; }

        [Required]
        public required string Description { get; set; }
    }
}
