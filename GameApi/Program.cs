using Core.Services;
using Core.Services.Extensions;
using GameApi.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("Default"));
});
builder.Services.AddOpenApi();

builder.Services.ExtendProblemDetails();
builder.Services.AddJwtAuthentication(configuration);

builder.Services.AddAppServices<Program>();

builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder();

builder.Services.AddCors(configuration, out string policyName);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(policyName);

app.MapControllers();

await app.RunAsync(configuration["Server:Url"]);