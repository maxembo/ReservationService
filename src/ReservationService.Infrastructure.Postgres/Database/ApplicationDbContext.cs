using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReservationService.Application.Database;
using ReservationService.Domain.Users;
using ReservationService.Domain.Venues;

namespace ReservationService.Infrastructure.Postgres.Database;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Venue> Venues => Set<Venue>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Seat> Seats => Set<Seat>();

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