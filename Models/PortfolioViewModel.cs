namespace StockPortfolioTracker.Models
{
    public class PortfolioViewModel
    {
        public Portfolio Portfolio { get; set; }

        public List<StockViewModel> Stocks { get; set; }

        public List<CardViewModel> Cards { get; set; }
    }

    public class StockViewModel
    {
        public int StockID { get; set; }

        public string Symbol { get; set; }

        public string CompanyName { get; set; }

        public int TotalShares { get; set; }

        public decimal CurrentPrice { get; set; }

        public decimal TotalValue { get; set; }
    }
}
