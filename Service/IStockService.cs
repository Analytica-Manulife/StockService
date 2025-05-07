using Newtonsoft.Json.Linq;
using StockMarketMicroservice.Models;
using StockMarketService.Model;

public interface IStockService
{
    Task<IEnumerable<Stock>> GetAllStocksAsync();
    Task<Stock> GetStockByTickerAsync(string ticker);
    Task UpdateAllStockDataAsync();
    
    Task<JObject> FetchTimeSeriesDataAsync(string ticker, string interval);
    IEnumerable<StockHistory> ParseTimeSeriesData(JObject rawData, string interval);

    Task<FullStockData> GetFullStockDataAsync(string ticker);
}