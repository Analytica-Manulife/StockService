using Microsoft.AspNetCore.Mvc;
using StockMarketMicroservice.Models;
using StockMarketService.Service;

    [Route("transections/[controller]")]
    [ApiController]
    public class StockTransactionController : ControllerBase
    {
        private readonly IStockTransactionService _stockTransactionService;

        public StockTransactionController(IStockTransactionService stockTransactionService)
        {
            _stockTransactionService = stockTransactionService;
        }

        // GET: api/StockTransaction/{accountId}
        [HttpGet("{accountId}")]
        public async Task<ActionResult<IEnumerable<StockTransaction>>> GetTransactionsByAccountIdAsync(Guid accountId)
        {
            try
            {
                var transactions = await _stockTransactionService.GetTransactionsByAccountIdAsync(accountId);
                if (transactions == null || !transactions.Any())
                {
                    return NotFound("No transactions found for the given account.");
                }
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
