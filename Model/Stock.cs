using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarketMicroservice.Models
{
    [Table("Stocks")]
    public class Stock
    {
        [Key]
        [Column("ticker")]
        [MaxLength(10)]
        public string Ticker { get; set; }

        [Column("company_name")]
        [MaxLength(255)]
        public string CompanyName { get; set; }

        [Column("yahoo_api_url")]
        public string YahooApiUrl { get; set; }

        [Column("last_price")]
        public decimal LastPrice { get; set; }

        [Column("last_updated")]
        public decimal LastUpdated { get; set; }

        [Column("stock_type")]
        [MaxLength(50)]
        public string StockType { get; set; }

        [Column("open_price")]
        public decimal OpenPrice { get; set; }

        [Column("high_price")]
        public decimal HighPrice { get; set; }

        [Column("low_price")]
        public decimal LowPrice { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("volume")]
        public long Volume { get; set; }

        [Column("latest_trading_day")]
        public DateTime LatestTradingDay { get; set; }

        [Column("change_amount")]
        public decimal ChangeAmount { get; set; }

        [Column("change_percent")]
        [MaxLength(10)]
        public string ChangePercent { get; set; }

        [Column("logo")]
        public string Logo { get; set; }
    }
}