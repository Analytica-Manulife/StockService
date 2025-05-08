using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockMarketService.Gateway;
using StockMarketService.Service;
using StockMarketMicroservice.Data;
using StockMarketMicroservice.Models;
using Microsoft.OpenApi.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using BudgetService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using StockMarketService.Data;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Configuration --------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

// Register Swagger for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StockMarketService API",
        Version = "v1",
        Description = "API for stock market data and portfolio management"
    });
});

// -------------------- Dependency Injection --------------------

// Register the database connection for Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register DbContext for Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register PortfolioService and IPortfolioService for dependency injection
builder.Services.AddScoped<IPortfolioService, PortfolioService>();

// Register other services for stock market operations (StockService, etc.)
builder.Services.AddScoped<IStockGateway, StockGateway>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockTransactionService, StockTransactionService>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
// -------------------- Build Application --------------------
var app = builder.Build();

// -------------------- Middleware Pipeline --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockMarketService API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
