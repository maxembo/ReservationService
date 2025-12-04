using Microsoft.Extensions.DependencyInjection;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;

namespace ReservationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        var scope = services.Scan(
            scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        return services;
    }
}