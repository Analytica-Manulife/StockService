namespace StockMarketService.Model;

public class StockHistory
{
    public DateTime Date { get; set; }          // Date of the historical data point
    public decimal Open { get; set; }           // Opening price
    public decimal High { get; set; }           // Highest price
    public decimal Low { get; set; }            // Lowest price
    public decimal Close { get; set; }          // Closing price
    public long Volume { get; set; }            // Volume traded on that day
}
