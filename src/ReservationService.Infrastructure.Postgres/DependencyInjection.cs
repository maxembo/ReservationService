using Microsoft.Extensions.DependencyInjection;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Infrastructure.Postgres.Database;
using ReservationService.Infrastructure.Postgres.Venues;

namespace ReservationService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped<ApplicationDbContext>();

        services.AddScoped<IVenuesRepository, EfCoreVenueRepository>();
        services.AddSingleton<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();

        services.AddScoped<ITransactionManager, TransactionManager>();

        //services.AddScoped<IVenuesRepository, NpgsqlVenueRepository>();

        //services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        return services;
    }
}