using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable All

namespace Core.Services;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AppServiceAttribute<TService>(
    ServiceLifetime lifetime = ServiceLifetime.Scoped) : Attribute where TService : class
{
    public ServiceLifetime Lifetime { get; } = lifetime;
}

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddAppServices<T>(this IServiceCollection services) 
        => services.AddAppServicesFromAssemblies(typeof(T).Assembly);
    public static IServiceCollection AddAppServicesFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var implementationTypes = assembly.GetTypes()
                .Where(type => type is { IsClass: true, IsAbstract: false })
                .Where(type => type.GetCustomAttributes()
                    .Any(attr => attr.GetType().IsGenericType &&
                                 attr.GetType().GetGenericTypeDefinition() == typeof(AppServiceAttribute<>)));

            foreach (var type in implementationTypes)
            {
                var attributes = type.GetCustomAttributes()
                    .Where(attr => attr.GetType().IsGenericType &&
                                   attr.GetType().GetGenericTypeDefinition() == typeof(AppServiceAttribute<>));

                foreach (var attr in attributes)
                {
                    var serviceType = attr.GetType().GetGenericArguments()[0];
                    var lifetime = (ServiceLifetime)attr.GetType()
                        .GetProperty(nameof(AppServiceAttribute<object>.Lifetime))!
                        .GetValue(attr)!;

                    if (!serviceType.IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException(
                            $"{type.FullName} does not implement {serviceType.FullName}");
                    }

                    services.Add(new ServiceDescriptor(serviceType, type, lifetime));
                }
            }
        }

        return services;
    }
}