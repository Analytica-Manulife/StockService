namespace StockMarketService.Model
{
    public class PortfolioStockDto
    {
        public Guid PortfolioId { get; set; }
        public Guid AccountId { get; set; }
        public string Ticker { get; set; }
        public decimal BuyPrice { get; set; }
        public DateTime BuyDate { get; set; }
        public int Quantity { get; set; }

        public string CompanyName { get; set; }
        public string YahooApiUrl { get; set; }
        public decimal LastPrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public string StockType { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal Price { get; set; }
        public long Volume { get; set; }
        public string LatestTradingDay { get; set; }
        public decimal ChangeAmount { get; set; }
        public string ChangePercent { get; set; }
        public string Logo { get; set; }
    }
}