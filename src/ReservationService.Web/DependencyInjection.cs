using ReservationService.Application;
using ReservationService.Infrastructure.Postgres;

namespace ReservationService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
        => services
            .AddWebDependencies()
            .AddInfrastructureDependencies()
            .AddApplicationDependencies();

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();

        return services;
    }
}