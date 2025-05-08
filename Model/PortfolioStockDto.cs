namespace StockMarketService.Model
{
    public class PortfolioStockDto
    {
        public Guid Portfolio_Id { get; set; }
        public Guid Account_Id { get; set; }
        public string Ticker { get; set; }
        public decimal Buy_Price { get; set; }
        public DateTime Buy_Date { get; set; }
        public int Quantity { get; set; }

        public string Company_Name { get; set; }
        public string Yahoo_Api_Url { get; set; }
        public decimal Last_Price { get; set; }
        public DateTime Last_Updated { get; set; }
        public string Stock_Type { get; set; }
        public decimal Open_Price { get; set; }
        public decimal High_Price { get; set; }
        public decimal Low_Price { get; set; }
        public decimal Price { get; set; }
        public long Volume { get; set; }
        public string Latest_Trading_Day { get; set; }
        public decimal Change_Amount { get; set; }
        public string Change_Percent { get; set; }
        public string Logo { get; set; }
    }
}