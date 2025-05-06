namespace StockMarketMicroservice.Data;

public class BuyStockRequest
{
    public Guid AccountId { get; set; }
    public string Ticker { get; set; }
    public int Quantity { get; set; }
    public decimal BuyPrice { get; set; }
}