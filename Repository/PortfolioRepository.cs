using Dapper;
using System.Data;
using StockMarketService.Data;
using StockMarketService.Model;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly IDbConnection _db;

    public PortfolioRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PortfolioStockDto>> GetPortfolioWithStocksAsync(Guid accountId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@account_id", accountId, DbType.Guid);

        var result = await _db.QueryAsync<PortfolioStockDto>(
            "GetPortfolioWithStocks",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }
}