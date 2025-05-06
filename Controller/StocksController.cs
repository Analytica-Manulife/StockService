using Microsoft.AspNetCore.Mvc;
using StockMarketService.Service;

namespace StockMarketService.Controller;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStocks()
    {
        var stocks = await _stockService.GetAllStocksAsync();
        return Ok(stocks);
    }

    [HttpGet("{ticker}")]
    public async Task<IActionResult> GetStockByTicker(string ticker)
    {
        var stock = await _stockService.GetStockByTickerAsync(ticker);
        if (stock == null) return NotFound();
        return Ok(stock);
    }
    
}