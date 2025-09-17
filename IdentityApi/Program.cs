using Core.Data;
using Core.Services;
using Core.Services.Extensions;
using FastEndpoints;
using FluentValidation;
using IdentityApi.Models;
using IdentityApi.Services;
using IdentityApi.Utilities;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder       = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.ExtendProblemDetails();
builder.Services.AddJwtAuthentication(configuration);

builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder();

builder.Services.Configure<HCaptchaSettings>(
    builder.Configuration.GetSection("HCaptcha"));

builder.Services.AddAppServices<Program>();
builder.Services.AddHttpClient<HCaptchaService>();

builder.Services.AddDatabaseConfiguration(configuration.GetConnectionString("Default")!);

builder.Services.AddIdentityCore<User>()
    .AddRoles<UserRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddApiEndpoints();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
builder.Services.AddCors(configuration, out string policyName);

var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine($"=== Incoming Request ===");
    Console.WriteLine($"Method: {context.Request.Method}");
    Console.WriteLine($"Path: {context.Request.Path}");
    Console.WriteLine($"QueryString: {context.Request.QueryString}");
    Console.WriteLine($"ContentType: {context.Request.ContentType}");
    Console.WriteLine($"Headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}"))}");
    
    if (context.Request.ContentLength > 0)
    {
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        Console.WriteLine($"Body: {body}");
    }
    Console.WriteLine("========================");
    
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();
app.UseCors(policyName);

app.UseFastEndpoints();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync(configuration["Server:Url"]);
