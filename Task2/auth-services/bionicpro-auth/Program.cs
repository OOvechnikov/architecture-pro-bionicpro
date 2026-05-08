using BionicProAuth.Middleware;
using BionicProAuth.Services;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using BionicProAuth.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<ITokenService, KeycloakTokenService>();
builder.Services.AddHttpClient<ReportService>();

var redisConnectionString = builder.Configuration["Redis"]!;
var hostingPort = builder.Configuration["Hosting.Port"]!;

var redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddDataProtection()
     ////   .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
	////.PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnectionString), "dp-keys")
      .PersistKeysToFileSystem(new DirectoryInfo(@"/keys")) // любой volume
	.SetApplicationName("bionicpro-auth");

builder.Services.Configure<YandexOAuthOptions>(
    builder.Configuration.GetSection("YandexOAuth"));
builder.Services.AddScoped<IUserRepository, RedisUserRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ISessionStore, RedisSessionStore>();

var app = builder.Build();

app.UseCors();
app.UseMiddleware<SessionMiddleware>();
app.MapControllers();

app.Run($"http://0.0.0.0:{hostingPort}");