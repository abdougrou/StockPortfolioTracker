using System;
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
    public class PortfoliosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PortfoliosController(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: Portfolios
        public async Task<IActionResult> Index()
        {
            // Fetch the portfolio with the User included
            var portfolio = await _db.Portfolio
                .Include(p => p.User)
                .Include(t => t.Transactions)
                .ThenInclude(t => t.Stock)
                .Include(t => t.Transactions)
                .ThenInclude(t => t.TransactionStatus)
                .FirstOrDefaultAsync(m => m.PortfolioID == 3);

            // Group transactions by stock and calculate total shares owned
            var portfolioStocks = portfolio.Transactions
                .GroupBy(t => t.StockID)
                .Select(g => new
                {
                    StockID = g.Key,
                    StockSymbol = g.First().Stock.Symbol,
                    StockCompanyName = g.First().Stock.CompanyName,
                    TotalShares = g.Sum(t => t.isBuy ? t.Quantity : -t.Quantity),
                    CurrentPrice = g.First().Stock.CurrentPrice,
                    TotalValue = g.Sum(t => t.isBuy ? t.Quantity : -t.Quantity) * g.First().Stock.CurrentPrice
                })
                .Where(s => s.TotalShares > 0)
                .ToList();

            if (portfolio != null)
            {
                portfolio.Transactions = portfolio.Transactions.OrderByDescending(t => t.TransactionDate).ToList();
            }

            // Calculate the total value of the portfolio by summing up each stock's TotalValue
            portfolio.TotalValue = portfolioStocks.Sum(s => s.TotalValue);

            // Total suspended transactions (assuming "Suspended" status is identified by a specific status ID or description)
            var pendingTransactions = portfolio.Transactions
                .Count(t => t.TransactionStatus.Description == "Pending");

            // Count how many unique stocks are in the portfolio
            var uniqueStocksCount = portfolioStocks.Count;

            // Additional database query example (e.g., total number of transactions)
            var totalTransactions = portfolio.Transactions.Count();

            // Prepare the card data
            var cards = new List<CardViewModel>
            {
                new CardViewModel { Header = "Total Value", Value = $"${portfolio.TotalValue}", BarColor = "border-left-success", HeaderColor = "text-success", Icon = "bi bi-currency-dollar"},
                new CardViewModel { Header = "Pending Transactions", Value = pendingTransactions.ToString(), BarColor = "border-left-warning", HeaderColor = "text-warning", Icon = "bi bi-hourglass-bottom" },
                new CardViewModel { Header = "Unique Stocks", Value = uniqueStocksCount.ToString(), BarColor = "border-left-info", HeaderColor = "text-info", Icon = "bi bi-graph-up-arrow" },
                new CardViewModel { Header = "Total Transactions", Value = totalTransactions.ToString(), BarColor = "border-left-info", HeaderColor = "text-info", Icon = "bi bi-currency-exchange" }
            };

            var viewModel = new PortfolioViewModel
            {
                Portfolio = portfolio,
                Stocks = portfolioStocks.Select(s => new StockViewModel
                {
                    StockID = s.StockID,
                    Symbol = s.StockSymbol,
                    CompanyName = s.StockCompanyName,
                    TotalShares = s.TotalShares,
                    CurrentPrice = s.CurrentPrice,
                    TotalValue = s.TotalValue
                }).ToList(),
                Cards = cards,
            };

            return View(viewModel);
        }

        // GET: Portfolios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _db.Portfolio
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PortfolioID == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // GET: Portfolios/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_db.Set<User>(), "UserID", "Email");
            return View();
        }

        // POST: Portfolios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PortfolioID,UserID,TotalValue,CreatedAt")] Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                _db.Add(portfolio);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_db.Set<User>(), "UserID", "Email", portfolio.UserID);
            return View(portfolio);
        }

        // GET: Portfolios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _db.Portfolio.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_db.Set<User>(), "UserID", "Email", portfolio.UserID);
            return View(portfolio);
        }

        // POST: Portfolios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PortfolioID,UserID,TotalValue,CreatedAt")] Portfolio portfolio)
        {
            if (id != portfolio.PortfolioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(portfolio);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortfolioExists(portfolio.PortfolioID))
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
            ViewData["UserID"] = new SelectList(_db.Set<User>(), "UserID", "Email", portfolio.UserID);
            return View(portfolio);
        }

        // GET: Portfolios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _db.Portfolio
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PortfolioID == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // POST: Portfolios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portfolio = await _db.Portfolio.FindAsync(id);
            if (portfolio != null)
            {
                _db.Portfolio.Remove(portfolio);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioExists(int id)
        {
            return _db.Portfolio.Any(e => e.PortfolioID == id);
        }
    }
}
