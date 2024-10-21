using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPortfolioTracker.Data;
using StockPortfolioTracker.Models;
using StockPortfolioTracker.Services;

namespace StockPortfolioTracker.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StockController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Stock> objStockList = _db.Stock;
            return View(objStockList);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Stock stock)
        {
            ModelState.Remove("Transactions");
            ModelState.Remove("StockPriceHistories");
            if (ModelState.IsValid)
            {
                _db.Stock.Add(stock);
                _db.SaveChanges();
                TempData["success"] = "Stock created successfully!";
                return RedirectToAction("Index");
            }

            return View(stock);
        }

        // GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) 
            {
                return NotFound();
            }

            var stockFromDB = _db.Stock.Find(id);

            if (stockFromDB == null)
            {
                return NotFound();
            }
            return View(stockFromDB);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _db.Stock
                .FirstOrDefaultAsync(m => m.StockID == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Stock stock)
        {
            ModelState.Remove("Transactions");
            ModelState.Remove("StockPriceHistories");
            if (ModelState.IsValid)
            {
                _db.Stock.Update(stock);
                _db.SaveChanges();
                TempData["success"] = $"Stock {stock.Symbol} updated successfully!";
                return RedirectToAction("Index");
            }

            return View(stock);
        }

        // GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var stockFromDB = _db.Stock.Find(id);

            if (stockFromDB == null)
            {
                return NotFound();
            }
            return View(stockFromDB);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _db.Stock.FindAsync(id);
            if (stock != null)
            {
                _db.Stock.Remove(stock);
            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Stock deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
