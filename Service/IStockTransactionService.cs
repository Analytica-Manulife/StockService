using StockMarketMicroservice.Models;

namespace StockMarketService.Service;

public interface IStockTransactionService
{
    Task<IEnumerable<StockTransaction>> GetTransactionsByAccountIdAsync(Guid accountId);
}