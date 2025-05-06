using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StockMarketMicroservice.Models;
using Newtonsoft.Json.Linq;

namespace StockMarketService.Gateway;

public class StockGateway : IStockGateway
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private const string ApiKey = "0D4M45OPUYZTNUTH"; 
        public StockGateway(IConfiguration configuration, IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
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
                LastUpdated = new DateTimeOffset(reader.GetDateTime(reader.GetOrdinal("last_updated"))).ToUnixTimeSeconds()
            };
        }

        
        public async Task UpdateStockAsync(Stock updatedStock)
        {
            // Sanitize and validate input
            updatedStock.OpenPrice = EnsureValidPrice(updatedStock.OpenPrice);
            updatedStock.HighPrice = EnsureValidPrice(updatedStock.HighPrice);
            updatedStock.LowPrice = EnsureValidPrice(updatedStock.LowPrice);
            updatedStock.Price = EnsureValidPrice(updatedStock.Price);
            updatedStock.ChangeAmount = EnsureValidPrice(updatedStock.ChangeAmount);
            updatedStock.Volume = EnsureValidVolume(updatedStock.Volume);

            var parameters = new[]
            {
                new SqlParameter("@ticker", updatedStock.Ticker),
                new SqlParameter("@company_name", updatedStock.CompanyName ?? (object)DBNull.Value),
                new SqlParameter("@open_price", updatedStock.OpenPrice),
                new SqlParameter("@high_price", updatedStock.HighPrice),
                new SqlParameter("@low_price", updatedStock.LowPrice),
                new SqlParameter("@price", updatedStock.Price),
                new SqlParameter("@volume", updatedStock.Volume),
                new SqlParameter("@latest_trading_day", updatedStock.LatestTradingDay),
                new SqlParameter("@change_amount", updatedStock.ChangeAmount),
                new SqlParameter("@change_percent", updatedStock.ChangePercent ?? (object)DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC UpdateStockData @ticker, @company_name, @open_price, @high_price, @low_price, @price, @volume, @latest_trading_day, @change_amount, @change_percent", parameters);
        }

        public async Task<Stock> FetchStockDataFromApiAsync(string ticker)
        {
            var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={ApiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content)["Global Quote"];
            if (json == null || !json.HasValues)
                return null;

            var latestTradingDay = DateTime.TryParse(json["07. latest trading day"]?.ToString(), out var parsedDate)
                ? parsedDate 
                : DateTime.UtcNow;

            var stock = new Stock
            {
                Ticker = json["01. symbol"]?.ToString(),
                LastPrice = EnsureValidPrice(decimal.Parse(json["05. price"]?.ToString() ?? "0")),
                OpenPrice = EnsureValidPrice(decimal.Parse(json["02. open"]?.ToString() ?? "0")),
                HighPrice = EnsureValidPrice(decimal.Parse(json["03. high"]?.ToString() ?? "0")),
                LowPrice = EnsureValidPrice(decimal.Parse(json["04. low"]?.ToString() ?? "0")),
                Price = EnsureValidPrice(decimal.Parse(json["05. price"]?.ToString() ?? "0")),
                Volume = EnsureValidVolume(long.Parse(json["06. volume"]?.ToString() ?? "0")),
                LatestTradingDay = latestTradingDay,
                ChangeAmount = EnsureValidPrice(decimal.Parse(json["09. change"]?.ToString() ?? "0")),
                ChangePercent = json["10. change percent"]?.ToString(),
                LastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            };

            return stock;
        }

        // Helper method to ensure the price is within the required precision and scale
        private decimal EnsureValidPrice(decimal price)
        {
            price = Math.Round(price, 2);
            return price > 99999999.99m ? 99999999.99m : price;
        }


// Helper method to ensure volume is a valid non-negative number
        private long EnsureValidVolume(long volume)
        {
            return volume < 0 ? 0 : volume; // Ensures volume can't be negative
        }
    }