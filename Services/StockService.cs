using StockPortfolioTracker.Data;
using StockPortfolioTracker.Models;

namespace StockPortfolioTracker.Services
{
    public class StockService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly Random _random;

        public StockService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _random = new Random();
        }

        public async Task LoadStockDataFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file at {filePath} could not be found.");
            }

            using (var reader = new StreamReader(filePath))
            {
                string line;
                bool skipFirstLine = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (skipFirstLine)
                    {
                        skipFirstLine = false;
                        continue;  // Skip first row
                    }

                    // Split the line to get symbol and company name
                    var data = line.Split('\t'); // Fields in my text file are tab-separated
                    if (data.Length != 2) continue;

                    string symbol = data[0].Trim();
                    string companyName = data[1].Trim();

                    // Generate random price between 1.00$ and 200.00$
                    decimal randomPrice = (decimal)_random.Next(100, 20000) / 100;

                    // Create new Stock entity
                    var stock = new Stock
                    {
                        Symbol = symbol,
                        CompanyName = companyName,
                        CurrentPrice = randomPrice
                    };

                    // Add to the database
                    _dbContext.Stock.Add(stock);
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
