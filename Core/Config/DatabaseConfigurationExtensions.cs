using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DatabaseConfigurationExtensions
{
    public static IServiceCollection AddDatabaseConfiguration<TDbContext>(
        this IServiceCollection services, string connectionString) 
        where TDbContext : DbContext
    {
        services.AddDbContextPool<TDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        return services;
    }
}