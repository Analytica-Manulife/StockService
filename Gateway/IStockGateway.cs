using StockMarketMicroservice.Models;

namespace StockMarketService.Gateway;

public interface IStockGateway
{
    Task<IEnumerable<Stock>> GetAllStocksAsync();
    Task<Stock> GetStockByTickerAsync(string ticker);
    Task<Stock> FetchStockDataFromApiAsync(string ticker);
    Task UpdateStockAsync(Stock stock);

}