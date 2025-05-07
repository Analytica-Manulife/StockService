using BudgetService.Services;
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
    
    [HttpPost("update-all")]
    public async Task<IActionResult> UpdateAllStocks()
    {
        await _stockService.UpdateAllStockDataAsync();
        return Ok(new { message = "Stock data updated manually." });
    }
    
    [HttpGet("{ticker}/history")]
    public async Task<IActionResult> GetStockHistory(string ticker, [FromQuery] string interval = "monthly")
    {
        var rawData = await _stockService.FetchTimeSeriesDataAsync(ticker, interval);
        if (rawData == null) return NotFound();

        var parsedData = _stockService.ParseTimeSeriesData(rawData, interval);
        return Ok(parsedData);
    }
    
    [HttpGet("{ticker}/full")]
    public async Task<IActionResult> GetFullStockData(string ticker)
    {
        var data = await _stockService.GetFullStockDataAsync(ticker);
        if (data == null || data.StockQuote == null) return NotFound();

        return Ok(data);
    }

    
}