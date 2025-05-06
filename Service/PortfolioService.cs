using System.Data;
using Dapper;
using StockMarketMicroservice.Models;
using StockMarketService.Service;

public class PortfolioService : IPortfolioService
    {
        private readonly IDbConnection _dbConnection;

        public PortfolioService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Get portfolio for a specific account (calls stored procedure)
        public async Task<IEnumerable<Portfolio>> GetPortfolioWithStocksAsync(Guid accountId)
        {
            var parameters = new { Account_Id = accountId };

            // Call the stored procedure GetPortfolioWithStocks
            return await _dbConnection.QueryAsync<Portfolio>(
                "GetPortfolioWithStocks",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

        // Buy stock and update portfolio (calls stored procedure)
        public async Task BuyStockAsync(Guid accountId, string ticker, int quantity, decimal buyPrice)
        {
            var parameters = new
            {
                Account_Id = accountId,
                Ticker = ticker,
                Quantity = quantity,
                Buy_price = buyPrice
            };

            // Call the stored procedure BuyStock
            await _dbConnection.ExecuteAsync(
                "BuyStock",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // Sell stock and update portfolio (calls stored procedure)
        public async Task SellStockAsync(Guid accountId, string ticker, int quantity, decimal sellPrice)
        {
            var parameters = new
            {
                Account_Id = accountId,
                Ticker = ticker,
                Quantity = quantity,
                Sell_Price = sellPrice
            };

            // Call the stored procedure SellStock
            await _dbConnection.ExecuteAsync(
                "SellStock",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // Get stock data (calls stored procedure)
        public async Task<Stock> GetStockDataAsync(string ticker)
        {
            var parameters = new { Ticker = ticker };

            // Call the stored procedure GetStockData
            return await _dbConnection.QuerySingleOrDefaultAsync<Stock>(
                "GetStockData",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // Update stock information (calls stored procedure)
        public async Task UpdateStockDataAsync(
            string ticker, 
            string companyName, 
            decimal openPrice, 
            decimal highPrice, 
            decimal lowPrice, 
            decimal price, 
            long volume, 
            DateTime latestTradingDay, 
            decimal changeAmount, 
            string changePercent)
        {
            var parameters = new
            {
                Ticker = ticker,
                CompanyName = companyName,
                OpenPrice = openPrice,
                HighPrice = highPrice,
                LowPrice = lowPrice,
                Price = price,
                Volume = volume,
                LatestTradingDay = latestTradingDay,
                ChangeAmount = changeAmount,
                ChangePercent = changePercent
            };

            // Call the stored procedure UpdateStock
            await _dbConnection.ExecuteAsync(
                "UpdateStock",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }