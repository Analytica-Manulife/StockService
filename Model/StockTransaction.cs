using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarketMicroservice.Models
{
    [Table("stock_transactions")]
    public class StockTransaction
    {
        [Key]
        [Column("transaction_id")]
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        [Column("account_id")]
        public Guid AccountId { get; set; }

        [Column("transaction_type")]
        [MaxLength(50)]
        public string TransactionType { get; set; } // BUY or SELL

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Column("ticker")]
        [MaxLength(50)]
        public string Ticker { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
    }
}