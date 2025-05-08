using StockMarketMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockMarketService.Model;

namespace StockMarketService.Service
{
    public interface IPortfolioService
    {
        // Get portfolio with stocks for a specific account
        Task<IEnumerable<Portfolio>> GetPortfolioWithStocksAsync(Guid accountId);

        // Buy stock and update portfolio
        Task BuyStockAsync(Guid accountId, string ticker, int quantity, decimal buyPrice);

        // Sell stock and update portfolio
        Task SellStockAsync(Guid accountId, string ticker, int quantity, decimal sellPrice);

        // Get stock data by ticker
        Task<Stock> GetStockDataAsync(string ticker);

        Task<IEnumerable<PortfolioStockDto>> GetPortfolioWithStockAccountsAsync(Guid accountId);

        // Update stock data
        Task UpdateStockDataAsync(
            string ticker, 
            string companyName, 
            decimal openPrice, 
            decimal highPrice, 
            decimal lowPrice, 
            decimal price, 
            long volume, 
            DateTime latestTradingDay, 
            decimal changeAmount, 
            string changePercent);
    }
}