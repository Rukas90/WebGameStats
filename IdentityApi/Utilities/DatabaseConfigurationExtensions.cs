using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Utilities;

internal static class DatabaseConfigurationExtensions
{
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        return services;
    }
}