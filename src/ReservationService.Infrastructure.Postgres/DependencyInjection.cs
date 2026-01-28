using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationService.Application.Database;
using ReservationService.Application.Events;
using ReservationService.Application.Reservations;
using ReservationService.Application.Venues;
using ReservationService.Infrastructure.Postgres.Database;
using ReservationService.Infrastructure.Postgres.Events;
using ReservationService.Infrastructure.Postgres.Reservations;
using ReservationService.Infrastructure.Postgres.Venues;

namespace ReservationService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ApplicationDbContext>(
            _ => new ApplicationDbContext(configuration.GetConnectionString("ReservationServiceDb")!));

        services.AddScoped<IReadDbContext, ApplicationDbContext>(
            _ => new ApplicationDbContext(configuration.GetConnectionString("ReservationServiceDb")!));

        services.AddScoped<IVenuesRepository, EfCoreVenueRepository>();
        services.AddScoped<ISeatsRepository, SeatsRepository>();
        services.AddScoped<IReservationsRepository, ReservationsRepository>();
        services.AddScoped<IEventsRepository, EventsRepository>();

        services.AddSingleton<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();

        services.AddScoped<ITransactionManager, TransactionManager>();

        //services.AddScoped<IVenuesRepository, NpgsqlVenueRepository>();

        //services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        return services;
    }
}