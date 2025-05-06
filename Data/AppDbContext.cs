using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockMarketMicroservice.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

public class AppDbContext : DbContext
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ILogger<AppDbContext> _logger;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IConfiguration configuration,
            ILogger<AppDbContext> logger)
            : base(options)
        {
            _configuration = configuration;
            _logger = logger;
            _logger.LogInformation("StockMarket AppDbContext initialized.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _logger.LogInformation("Configuring AppDbContext for Azure SQL...");

            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString, sqlOpts =>
                {
                    sqlOpts.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

                _logger.LogInformation("SQL Server (Azure) configured with DefaultConnection.");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships and constraints for Portfolio and StockTransaction
            modelBuilder.Entity<Portfolio>()
                .HasKey(p => new { p.Account_Id, p.Ticker });

            modelBuilder.Entity<Portfolio>()
                .HasIndex(p => new { p.Account_Id, p.Ticker })
                .IsUnique();

            modelBuilder.Entity<StockTransaction>()
                .HasKey(t => t.TransactionId); // Assuming TransactionId is the primary key

            modelBuilder.Entity<StockTransaction>()
                .HasOne<Portfolio>()
                .WithMany(p => p.StockTransactions)
                .HasForeignKey(t => new { t.AccountId, t.Ticker })
                .OnDelete(DeleteBehavior.Cascade); // Delete transactions when portfolio is deleted

            modelBuilder.Entity<StockTransaction>()
                .Property(t => t.TransactionType)
                .HasConversion<string>(); // Assuming TransactionType is an enum

            // Ensure stock symbols are case-insensitive for querying
            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Ticker)
                .IsUnique()
                .HasDatabaseName("IX_Stock_Ticker")
                .IsClustered();

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            _logger.LogInformation("Saving changes to the stock market database...");
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to the stock market database (async)...");
            return base.SaveChangesAsync(cancellationToken);
        }
    }