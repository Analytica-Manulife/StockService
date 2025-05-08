using StockMarketService.Model;


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockMarketMicroservice.Models;
using StockMarketService.Model;

namespace StockMarketService.Data
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<PortfolioStockDto>> GetPortfolioWithStocksAsync(Guid accountId);
        
   
    }
}
