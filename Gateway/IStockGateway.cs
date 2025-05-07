using Newtonsoft.Json.Linq;
using StockMarketMicroservice.Models;
using StockMarketService.Model;

namespace StockMarketService.Gateway;

public interface IStockGateway
{
    Task<IEnumerable<Stock>> GetAllStocksAsync();
    Task<Stock> GetStockByTickerAsync(string ticker);
    Task<Stock> FetchStockDataFromApiAsync(string ticker);
    Task UpdateStockAsync(Stock stock1,Stock stock);

    Task<JObject> FetchTimeSeriesDataAsync(string ticker, string interval = "monthly");
    List<StockHistory> ParseTimeSeriesData(JObject json, string interval);
}