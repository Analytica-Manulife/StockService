using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarketMicroservice.Models
{
    [Table("portfolio")]
    public class Portfolio
    {
        [Key]
        [Column("portfolio_id")]
        public Guid PortfolioId { get; set; } = Guid.NewGuid();

        [Column("account_id")]
        public Guid Account_Id { get; set; }

        [Column("ticker")]
        [MaxLength(50)]
        public string Ticker { get; set; }

        [Column("buy_price")]
        public decimal Buy_price { get; set; }

        [Column("buy_date")]
        public DateTime BuyDate { get; set; } = DateTime.UtcNow;

        [Column("quantity")]
        public int Quantity { get; set; }
        
        public ICollection<StockTransaction> StockTransactions { get; set; }

    }
}