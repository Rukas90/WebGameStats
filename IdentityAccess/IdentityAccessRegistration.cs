using Microsoft.Extensions.DependencyInjection;

namespace IdentityAccess;

public static class IdentityAccessRegistration
{
    public static IServiceCollection AddIdentityAccess(this IServiceCollection services)
    {
        return services.AddScoped<IUserDataService, UserDataService>();
    }
}