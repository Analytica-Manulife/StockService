using StockMarketMicroservice.Models;

namespace StockMarketService.Service;

public interface IStockService
{
    Task<IEnumerable<Stock>> GetAllStocksAsync();
    Task<Stock> GetStockByTickerAsync(string ticker);
}

