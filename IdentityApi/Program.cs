using Core;
using Core.Data;
using Core.Middleware;
using Core.Services;
using Core.Services.Extensions;
using FastEndpoints;
using FluentValidation;
using IdentityApi.Models;
using IdentityApi.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder       = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();

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

builder.Services.AddAuthorizationBuilder();

builder.Services.Configure<HCaptchaSettings>(
    builder.Configuration.GetSection("HCaptcha"));

builder.Services.AddAppServices<Program>();
builder.Services.AddHttpClient<HCaptchaService>();

builder.Services.AddDatabaseConfiguration<UserDbContext>
    (configuration.GetConnectionString("Default")!);

builder.Services.AddIdentityCore<User>()
    .AddRoles<UserRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddApiEndpoints();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
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
app.UseCors(policyName);

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgeryFE();
app.UseFastEndpoints();

await app.RunAsync(configuration["Server:Url"]);
