using Microsoft.AspNetCore.Mvc;
using StockMarketMicroservice.Data;
using StockMarketMicroservice.Models;
using StockMarketService.Model;
using StockMarketService.Service;

namespace StockMarketService.Controller;

[Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        // GET: api/Portfolio/{accountId}
        [HttpGet("{accountId}")]
        public async Task<ActionResult<IEnumerable<Portfolio>>> GetPortfolioWithStocksAsync(Guid accountId)
        {
            try
            {
                var portfolio = await _portfolioService.GetPortfolioWithStocksAsync(accountId);
                if (portfolio == null || portfolio.Equals(0))
                {
                    return NotFound("Portfolio not found for the given account.");
                }
                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Portfolio/BuyStock
        [HttpPost("BuyStock")]
        public async Task<ActionResult> BuyStockAsync([FromBody] BuyStockRequest request)
        {
            try
            {
                await _portfolioService.BuyStockAsync(request.AccountId, request.Ticker, request.Quantity, request.BuyPrice);
                return Ok("Stock purchase successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Portfolio/SellStock
        [HttpPost("SellStock")]
        public async Task<ActionResult> SellStockAsync([FromBody] SellStockRequest request)
        {
            try
            {
                await _portfolioService.SellStockAsync(request.AccountId, request.Ticker, request.Quantity, request.SellPrice);
                return Ok("Stock sale successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Portfolio/GetStockData/{ticker}
        [HttpGet("GetStockData/{ticker}")]
        public async Task<ActionResult<Stock>> GetStockDataAsync(string ticker)
        {
            try
            {
                var stock = await _portfolioService.GetStockDataAsync(ticker);
                if (stock == null)
                {
                    return NotFound("Stock data not found for the given ticker.");
                }
                return Ok(stock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateStockData")]
        public async Task<ActionResult> UpdateStockDataAsync([FromBody] UpdateStockRequest request)
        {
            try
            {
                await _portfolioService.UpdateStockDataAsync( 
                    request.Ticker,
                    request.CompanyName,
                    request.OpenPrice,
                    request.HighPrice,
                    request.LowPrice,
                    request.Price,
                    request.Volume,
                    request.LatestTradingDay,
                    request.ChangeAmount,
                    request.ChangePercent
                );
                return Ok("Stock data updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{accountId}/portfolio")]
        public async Task<ActionResult<IEnumerable<PortfolioStockDto>>> GetPortfolioWithStocksAccountAsync(Guid accountId)
        {
            try
            {
                var portfolio = await _portfolioService.GetPortfolioWithStockAccountsAsync(accountId);
                if (portfolio == null || !portfolio.Any())
                {
                    return NotFound("Portfolio not found for the given account.");
                }
                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }