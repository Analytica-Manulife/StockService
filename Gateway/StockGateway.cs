using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StockMarketMicroservice.Models;
using Newtonsoft.Json.Linq;
using StockMarketService.Model;

namespace StockMarketService.Gateway;

public class StockGateway : IStockGateway
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private const string ApiKey = "0D4M45OPUYZTNUTH"; 
        private const string ApiKey2 = "09UPT1SFXZOI0CBC"; 

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
                YahooApiUrl = reader["yahoo_api_url"]?.ToString(),
                LastPrice = reader["last_price"] != DBNull.Value ? reader.GetDecimal(reader.GetOrdinal("last_price")) : 0,
                OpenPrice = reader.GetDecimal(reader.GetOrdinal("open_price")),
                HighPrice = reader.GetDecimal(reader.GetOrdinal("high_price")),
                LowPrice = reader.GetDecimal(reader.GetOrdinal("low_price")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Volume = reader.GetInt64(reader.GetOrdinal("volume")),
                LatestTradingDay = reader.GetDateTime(reader.GetOrdinal("latest_trading_day")),
                ChangeAmount = reader.GetDecimal(reader.GetOrdinal("change_amount")),
                ChangePercent = reader["change_percent"].ToString(),
                StockType = reader["stock_type"]?.ToString(),
                LastUpdated = new DateTimeOffset(reader.GetDateTime(reader.GetOrdinal("last_updated"))).ToUnixTimeSeconds(),
                Logo = reader["logo"]?.ToString()
            };
        }


        
        public async Task UpdateStockAsync(Stock orignalStock, Stock updatedStock)
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
                new SqlParameter("@company_name", orignalStock.CompanyName ?? (object)DBNull.Value),
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
        
        public async Task<JObject> FetchTimeSeriesDataAsync(string ticker, string interval = "monthly")
        {
            string function = interval.ToLower() switch
            {
                "daily" => "TIME_SERIES_DAILY",
                "weekly" => "TIME_SERIES_WEEKLY",
                _ => "TIME_SERIES_MONTHLY"
            };

            var url = $"https://www.alphavantage.co/query?function={function}&symbol={ticker}&apikey={ApiKey2}";
            Console.WriteLine($"[INFO] Requesting data from URL: {url}");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ERROR] Failed to fetch data. Status Code: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] Raw JSON Response: {content}");

            var json = JObject.Parse(content);

            if (json["Error Message"] != null)
            {
                Console.WriteLine("[ERROR] API returned error message: " + json["Error Message"]);
                return null;
            }

            if (json["Note"] != null)
            {
                Console.WriteLine("[WARNING] API limit notice: " + json["Note"]);
                return null;
            }

            Console.WriteLine("[INFO] Successfully fetched and parsed JSON response.");
            return json;
        }

        
        public List<StockHistory> ParseTimeSeriesData(JObject json, string interval)
        {
            var seriesKey = interval switch
            {
                "daily" => "Time Series (Daily)",
                "weekly" => "Weekly Time Series",
                _ => "Monthly Time Series"
            };

            var data = json[seriesKey];
            if (data == null) return new List<StockHistory>();

            var history = new List<StockHistory>();

            foreach (var entry in data.Children<JProperty>())
            {
                var date = DateTime.Parse(entry.Name);
                var values = entry.Value;

                history.Add(new StockHistory
                {
                    Date = date,
                    Open = decimal.Parse(values["1. open"].ToString()),
                    High = decimal.Parse(values["2. high"].ToString()),
                    Low = decimal.Parse(values["3. low"].ToString()),
                    Close = decimal.Parse(values["4. close"].ToString()),
                    Volume = long.Parse(values["5. volume"].ToString())
                });
            }

            return history.OrderByDescending(h => h.Date).ToList();
        }
        
        public async Task<JObject> FetchCompanyOverviewAsync(string ticker)
        {
            var url = $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={ticker}&apikey={ApiKey2}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
        
        public async Task<FullStockData> GetFullStockDataAsync(string ticker)
        {
            var stock = await FetchStockDataFromApiAsync(ticker);
            var historyJson = await FetchTimeSeriesDataAsync(ticker);
            var history = ParseTimeSeriesData(historyJson, "monthly");
            var overview = await FetchCompanyOverviewAsync(ticker);

            return new FullStockData
            {
                StockQuote = stock,
                History = history,
                Overview = overview
            };
        }

    }