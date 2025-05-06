using System.Data;
using Dapper;
using StockMarketMicroservice.Models;

namespace StockMarketService.Service;

public class StockTransactionService : IStockTransactionService
{
    private readonly IDbConnection _dbConnection;

    public StockTransactionService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    // Get transactions by AccountId
    public async Task<IEnumerable<StockTransaction>> GetTransactionsByAccountIdAsync(Guid accountId)
    {
        try
        {
            var parameters = new { AccountId = accountId };
            return await _dbConnection.QueryAsync<StockTransaction>(
                "SELECT * FROM stock_transactions WHERE account_id = @AccountId",
                parameters,
                commandType: CommandType.Text
            );
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching transactions for the given account.", ex);
        }
    }
}