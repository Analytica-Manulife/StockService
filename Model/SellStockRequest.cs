namespace StockMarketService.Model;

public class SellStockRequest
{
    public Guid AccountId { get; set; }
    public string Ticker { get; set; }
    public int Quantity { get; set; }
    public decimal SellPrice { get; set; }
}