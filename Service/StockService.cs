using BudgetService.Services;
using StockMarketMicroservice.Models;
using StockMarketService.Gateway;

namespace StockMarketService.Service;

public class StockService : IStockService
{
    private readonly IStockGateway _stockGateway;

    public StockService(IStockGateway stockGateway)
    {
        _stockGateway = stockGateway;
    }

    public Task<IEnumerable<Stock>> GetAllStocksAsync()
    {
        return _stockGateway.GetAllStocksAsync();
    }

    public Task<Stock> GetStockByTickerAsync(string ticker)
    {
        return _stockGateway.GetStockByTickerAsync(ticker);
    }
    public async Task UpdateAllStockDataAsync()
    {
        var stocks = await _stockGateway.GetAllStocksAsync();
        foreach (var stock in stocks)
        {
            var updatedStock = await _stockGateway.FetchStockDataFromApiAsync(stock.Ticker);
            if (updatedStock != null)
            {
                await _stockGateway.UpdateStockAsync(updatedStock); // Persist to DB
            }
        }
    }

}