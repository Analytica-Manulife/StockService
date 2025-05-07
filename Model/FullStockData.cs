using Newtonsoft.Json.Linq;
using StockMarketMicroservice.Models;

namespace StockMarketService.Model;

public class FullStockData
{
    public Stock StockQuote { get; set; }
    public List<StockHistory> History { get; set; }
    public JObject Overview { get; set; } // or create a typed model
}
