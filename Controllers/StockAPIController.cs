using Microsoft.AspNetCore.Mvc;
using StockPortfolioTracker.Services;

namespace StockPortfolioTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockAPIController : Controller
    {
        private readonly StockService _stockService;

        public StockAPIController(StockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost("import-stocks")]
        public async Task<IActionResult> ImportStocks([FromBody] string filePath)
        {
            Console.WriteLine($"Received filePath: {filePath}");

            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest("File path is required.");
            }

            try
            {
                await _stockService.LoadStockDataFromFile(filePath);
                TempData["success"] = "Stock data successfully imported!";
                return Ok("Stock data successfully imported!");
            }
            catch (FileNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(500, $"Internal server error: {exception.Message}");
            }
        }
    }
}
