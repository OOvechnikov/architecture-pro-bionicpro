using ReportService.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var redisConnectionString = builder.Configuration["Redis"]!;
var hostingPort = builder.Configuration["Hosting.Port"]!;

builder.Services.AddScoped<ClickhouseRepository>();

var app = builder.Build();

app.UseCors();
app.MapControllers();

app.Run($"http://0.0.0.0:{hostingPort}");