using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReservationService.Application.Database;
using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Users;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Infrastructure.Postgres.Database;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Venue> Venues => Set<Venue>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Seat> Seats => Set<Seat>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<Event> Events => Set<Event>();

    public async Task<UnitResult<Error>> SaveChangesResultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            return GeneralErrors.Database(null, ex.Message);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("ReservationServiceDb"));

        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();

        //optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.UseLoggerFactory(UseCreateFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private ILoggerFactory UseCreateFactory()
        => LoggerFactory.Create(configure => configure.AddConsole());
}