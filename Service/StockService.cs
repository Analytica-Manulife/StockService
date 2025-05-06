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
    
}