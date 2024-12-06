using StackExchange.Redis;
using ExchangeRateAPI.Services;
using ExchangeRateAPI.Models;
using ExchangeRateAPI.Repositories;
using Serilog;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379, abortConnect=false")
);

builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ExternalApis"));
builder.Services.AddHttpClient<IExchangeRateFetcher, ExchangeRateFetcher>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddSingleton<IRedisRepository, RedisRepository>();

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles( new StaticFileOptions{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "browser"))
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html", new StaticFileOptions{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory() , "wwwroot", "browser"))
});

var url = "http://localhost:5005";
Process.Start(new ProcessStartInfo{
    FileName = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
    Arguments = url,
    UseShellExecute = true
});

app.Run();

