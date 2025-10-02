using Core;
using Core.Middleware;
using Core.Services;
using Core.Services.Extensions;
using GameApi.Data;
using Microsoft.AspNetCore.DataProtection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddDatabaseConfiguration<GameDbContext>
    (configuration.GetConnectionString("mkdb")!);

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
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(policyName);
app.UseAntiforgery();

app.MapControllers();

app.MapGet("/v1/ping", () => Results.Ok("pong"));

if (app.Environment.IsDevelopment())
{
    await app.RunAsync(configuration["Server:Url"]);
}
else
{
    await app.RunAsync();
}