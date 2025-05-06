using System.Data;
using Microsoft.Data.SqlClient;
using StockMarketMicroservice.Models;

namespace StockMarketService.Gateway;

public class StockGateway : IStockGateway
    {
        private readonly IConfiguration _configuration;

        public StockGateway(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            var stocks = new List<Stock>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("SELECT * FROM Stocks", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                stocks.Add(MapReaderToStock(reader));
            }

            return stocks;
        }

        public async Task<Stock> GetStockByTickerAsync(string ticker)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("GetStockData", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@ticker", ticker);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapReaderToStock(reader) : null;
        }

        private Stock MapReaderToStock(SqlDataReader reader)
        {
            return new Stock
            {
                Ticker = reader["ticker"].ToString(),
                CompanyName = reader["company_name"].ToString(),
                OpenPrice = reader.GetDecimal(reader.GetOrdinal("open_price")),
                HighPrice = reader.GetDecimal(reader.GetOrdinal("high_price")),
                LowPrice = reader.GetDecimal(reader.GetOrdinal("low_price")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Volume = reader.GetInt64(reader.GetOrdinal("volume")),
                LatestTradingDay = reader.GetDateTime(reader.GetOrdinal("latest_trading_day")),
                ChangeAmount = reader.GetDecimal(reader.GetOrdinal("change_amount")),
                ChangePercent = reader["change_percent"].ToString(),
                LastUpdated = reader.GetDecimal(reader.GetOrdinal("last_updated"))
            };
        }
        
        
    }