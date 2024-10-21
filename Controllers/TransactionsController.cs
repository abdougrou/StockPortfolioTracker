using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockPortfolioTracker.Data;
using StockPortfolioTracker.Models;

namespace StockPortfolioTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TransactionsController(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _db.Transaction.Include(t => t.Portfolio).Include(t => t.Stock).Include(t => t.TransactionStatus);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _db.Transaction
                .Include(t => t.Portfolio)
                .Include(t => t.Stock)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        [HttpGet]
        public async Task<IActionResult> GetStockDetails(int stockId, int portfolioId)
        {
            // Fetch the transaction details for the specific portfolio and stock
            var ownedStock = await _db.Transaction
                .Where(t => t.PortfolioID == portfolioId && t.StockID == stockId)
                .GroupBy(t => t.StockID)
                .Select(g => new
                {
                    CurrentPrice = g.First().Stock.CurrentPrice, // Assuming current price is in the Stock model
                    TotalShares = g.Sum(t => t.isBuy ? t.Quantity : -t.Quantity) // Total shares owned
                })
                .FirstOrDefaultAsync();

            if (ownedStock == null)
            {
                return NotFound();
            }

            return Json(new { price = ownedStock.CurrentPrice, shares = ownedStock.TotalShares });
        }

        [HttpGet]
        public async Task<IActionResult> GetStockPrices(int stockId)
        {
            // Fetch the stock price
            var stock = await _db.Stock
                .Where(s => s.StockID == stockId)
                .Select(t => new
                {
                    t.StockID,
                    t.CurrentPrice
                })
                .FirstOrDefaultAsync();

            if (stock == null)
            {
                return NotFound();
            }

            return Json(new { price = stock.CurrentPrice });
        }

        // GET: Transactions/Buy
        public IActionResult Buy()
        {
            ViewData["PortfolioID"] = new SelectList(_db.Set<Portfolio>(), "PortfolioID", "PortfolioID");
            ViewData["StockID"] = new SelectList(_db.Stock, "StockID", "CompanyName", "CurrentPrice");
            return View();
        }

        // POST: Transactions/Buy
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([Bind("TransactionID,PortfolioID,StockID,Quantity,isBuy,TransactionStatusID,PriceAtTransaction,TransactionDate")] Transaction transaction)
        {
            ModelState.Remove("Portfolio");
            ModelState.Remove("Stock");
            ModelState.Remove("TransactionStatus");
            if (ModelState.IsValid)
            {
                // Update Portfolio Total Value
                var portfolio = await _db.Portfolio.FindAsync(transaction.PortfolioID);
                portfolio.TotalValue += transaction.Quantity * transaction.PriceAtTransaction;
                _db.Portfolio.Update(portfolio);

                // Fill null attributes
                transaction.isBuy = true;
                transaction.Portfolio = portfolio;
                transaction.Stock = await _db.Stock.FindAsync(transaction.StockID)!;
                _db.Add(transaction);
                await _db.SaveChangesAsync();
                TempData["success"] = $"Stock {transaction.Stock.Symbol} is bought successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PortfolioID"] = new SelectList(_db.Set<Portfolio>(), "PortfolioID", "PortfolioID", transaction.PortfolioID);
            ViewData["StockID"] = new SelectList(_db.Stock, "StockID", "CompanyName", transaction.StockID);
            return View(transaction);
        }

        // GET: Transactions/Create
        public async Task<IActionResult> Sell()
        {
            var portfolio = await _db.Portfolio
                .Include(p => p.User)
                .Include(t => t.Transactions)
                .ThenInclude(t => t.Stock)
                .Include(t => t.Transactions)
                .ThenInclude(t => t.TransactionStatus)
                .FirstOrDefaultAsync(m => m.PortfolioID == 3);

            // Get transactions related to the portfolio
            var ownedStocks = portfolio.Transactions // Filter by portfolio
                .GroupBy(t => t.StockID) // Group by stock
                .Select(g => new
                {
                    StockID = g.Key,
                    TotalShares = g.Sum(t => t.isBuy ? t.Quantity : -t.Quantity), // Calculate net shares owned
                    Stock = g.First().Stock // Get the stock details
                })
                .Where(s => s.TotalShares > 0) // Only include stocks where the user owns shares
                .Select(s => s.Stock) // Select the stock object
                .ToList();

            ViewData["PortfolioID"] = new SelectList(_db.Set<Portfolio>(), "PortfolioID", "PortfolioID");
            ViewData["StockID"] = new SelectList(ownedStocks, "StockID", "CompanyName", "CurrentPrice");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sell([Bind("TransactionID,PortfolioID,StockID,Quantity,isBuy,PriceAtTransaction,TransactionDate")] Transaction transaction)
        {
            ModelState.Remove("Portfolio");
            ModelState.Remove("Stock");
            ModelState.Remove("TransactionStatus");
            if (ModelState.IsValid)
            {
                // Update Portfolio Total Value
                var portfolio = await _db.Portfolio.FindAsync(transaction.PortfolioID);
                portfolio.TotalValue -= transaction.Quantity * transaction.PriceAtTransaction;
                _db.Portfolio.Update(portfolio);

                // Fill null attributes
                transaction.isBuy = false;
                transaction.Portfolio = portfolio;
                transaction.Stock = await _db.Stock.FindAsync(transaction.StockID)!;
                _db.Add(transaction);
                await _db.SaveChangesAsync();
                TempData["success"] = $"Stock {transaction.Stock.Symbol} is sold successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PortfolioID"] = new SelectList(_db.Set<Portfolio>(), "PortfolioID", "PortfolioID", transaction.PortfolioID);
            ViewData["StockID"] = new SelectList(_db.Stock, "StockID", "CompanyName", transaction.StockID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _db.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["PortfolioID"] = new SelectList(_db.Set<Portfolio>(), "PortfolioID", "PortfolioID", transaction.PortfolioID);
            ViewData["StockID"] = new SelectList(_db.Stock, "StockID", "CompanyName", transaction.StockID);
            ViewData["TransactionStatusID"] = new SelectList(_db.TransactionStatus, "TransactionStatusID", "Description", transaction.TransactionStatusID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,PortfolioID,StockID,Quantity,PriceAtTransaction,isBuy,TransactionStatusID,TransactionDate")] Transaction transaction)
        {
            if (id != transaction.TransactionID)
            {
                return NotFound();
            }
            ModelState.Remove("Portfolio");
            ModelState.Remove("Stock");
            ModelState.Remove("TransactionStatus");
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(transaction);
                    await _db.SaveChangesAsync();
                    TempData["success"] = "Transaction updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PortfolioID"] = new SelectList(_db.Set<Portfolio>(), "PortfolioID", "PortfolioID", transaction.PortfolioID);
            ViewData["StockID"] = new SelectList(_db.Stock, "StockID", "CompanyName", transaction.StockID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _db.Transaction
                .Include(t => t.Portfolio)
                .Include(t => t.Stock)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _db.Transaction.FindAsync(id);
            if (transaction != null)
            {
                _db.Transaction.Remove(transaction);
            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Transaction deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _db.Transaction.Any(e => e.TransactionID == id);
        }
    }
}
