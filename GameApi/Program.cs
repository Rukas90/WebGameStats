using System.Threading.RateLimiting;
using Core;
using Core.Middleware;
using Core.Services;
using Core.Services.Extensions;
using GameApi.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddDatabaseConfiguration<GameDbContext>
    (configuration.GetConnectionString("Default")!);

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit          = 6;
        options.Window               = TimeSpan.FromSeconds(14);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit           = 6;
    });
});
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name         = Constants.Tokens.CSRF_TOKEN_COOKIE_NAME;
    options.Cookie.SameSite     = SameSiteMode.Strict;
    options.Cookie.HttpOnly     = false;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.HeaderName          = Constants.Tokens.CSRF_TOKEN_HEADER_NAME;
});
builder.Services.AddOpenApi();

builder.Services.ExtendProblemDetails();
builder.Services.AddJwtAuthentication(configuration);

builder.Services.AddAppServices<Program>();

builder.Services.AddAuthorizationBuilder();

builder.Services.AddCors(configuration, out string policyName);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(path: "../SharedKeys"))
    .SetApplicationName("WebGameStats");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseIncomingRequestLogger();
}
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.UseCors(policyName);
app.UseAntiforgery();

app.MapControllers();

app.MapGet("/ping", () => Results.Ok("pong"));
app.MapGet("/", () => Results.Ok("Game Api is running"));

if (app.Environment.IsDevelopment())
{
    await app.RunAsync(configuration["Server:Url"]);
    return;
}
await app.RunAsync();