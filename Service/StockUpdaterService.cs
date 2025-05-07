namespace BudgetService.Services;

public class StockUpdaterService : BackgroundService
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockUpdaterService> _logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(2);

    public StockUpdaterService(IStockService stockService, ILogger<StockUpdaterService> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // _logger.LogInformation("Stock Updater Service started.");
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     try
        //     {
        //         await _stockService.UpdateAllStockDataAsync();
        //         _logger.LogInformation("Stock data updated at: {time}", DateTime.Now);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error updating stock data");
        //     }
        //
        //     await Task.Delay(_updateInterval, stoppingToken);
        // }
    }
}