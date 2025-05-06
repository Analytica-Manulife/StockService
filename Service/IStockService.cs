using StockMarketMicroservice.Models;

namespace BudgetService.Services;

public interface IStockService
{
    Task<IEnumerable<Stock>> GetAllStocksAsync();
    Task<Stock> GetStockByTickerAsync(string ticker);
    Task UpdateAllStockDataAsync(); // <-- New Method
}
