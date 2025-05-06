namespace StockMarketService.Model;

public class UpdateStockRequest
{
    public string Ticker { get; set; }
    public string CompanyName { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal Price { get; set; }
    public long Volume { get; set; }
    public DateTime LatestTradingDay { get; set; }
    public decimal ChangeAmount { get; set; }
    public string ChangePercent { get; set; }
}
