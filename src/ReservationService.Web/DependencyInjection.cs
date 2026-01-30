using ReservationService.Application;
using ReservationService.Infrastructure.Postgres;

namespace ReservationService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddWebDependencies()
            .AddInfrastructureDependencies(configuration)
            .AddApplicationDependencies();

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddLogging();

        return services;
    }
}