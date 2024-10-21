using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPortfolioTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        public int PortfolioID { get; set; }

        [ForeignKey("PortfolioID")]
        public Portfolio Portfolio { get; set; }

        public int StockID { get; set; }

        [ForeignKey("StockID")]
        public Stock Stock { get; set; }

        public int? TransactionStatusID { get; set; }

        [ForeignKey("TransactionStatusID")]
        public TransactionStatus TransactionStatus { get; set; }

        public int Quantity { get; set; }

        [DisplayName("Transaction Type")]
        public bool isBuy { get; set; }  // 0: Sell Transaction, 1: Buy Transaction


        [DisplayName("Price At Transaction")]
        public decimal PriceAtTransaction { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
