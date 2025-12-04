using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Domain.Venues;
using ReservationService.Infrastructure.Postgres.Database;
using Shared;

namespace ReservationService.Infrastructure.Postgres.Venues;

public class EfCoreVenueRepository : IVenuesRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<EfCoreVenueRepository> _logger;
    private IVenuesRepository _venuesRepositoryImplementation;

    public EfCoreVenueRepository(ApplicationDbContext dbContext, ILogger<EfCoreVenueRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Venues.AddAsync(venue, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return venue.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to insert Venue.");

            return GeneralErrors.Database(ex.Message);
        }
    }

    public async Task<Result<Guid, Error>> UpdateNameAsync(
        VenueId venueId, VenueName venueName, CancellationToken cancellationToken = default)
    {
        // var sql = "UPDATE venues SET name = @Name Where id = @Id";
        //
        // await _dbContext.Database.ExecuteSqlRawAsync(
        //     sql,
        //     new NpgsqlParameter("@Name", venueName.Name),
        //     new NpgsqlParameter("@Id", venueId.Value));

        await _dbContext.Venues
            .Where(v => v.Id == venueId)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(v => v.VenueName.Name, venueName.Name)
                    .SetProperty(v => v.SeatsLimit, 100), cancellationToken);

        return venueId.Value;
    }

    public async Task<UnitResult<Error>> UpdateNameByPrefixAsync(
        string prefix, VenueName name, CancellationToken cancellationToken = default)
    {
        await _dbContext.Venues
            .Where(v => v.VenueName.Prefix.StartsWith(prefix))
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(v => v.VenueName.Name, name.Name)
                    .SetProperty(v => v.VenueName.Prefix, prefix), cancellationToken);

        return UnitResult.Success<Error>();
    }

    // public async Task SaveAsync(CancellationToken cancellationToken = default)
    // {
    //     var entry = _dbContext.ChangeTracker.Entries();
    //
    //     await _dbContext.SaveChangesAsync(cancellationToken);
    // }

    public async Task<Result<Venue, Error>> GetByIdAsync(VenueId id, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (venue is null)
            return GeneralErrors.NotFound(id.Value, "venue id");

        return venue;
    }

    public async Task<Result<Venue, Error>> GetSeatsByIdAsync(VenueId id, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .Include(v => v.Seats)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (venue is null)
            return GeneralErrors.NotFound(id.Value, "venue id");

        return venue;
    }

    public async Task<Result<Guid, Error>> UpdateAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        await _dbContext.Venues
            .Where(v => v.Id == venue.Id)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(v => v.VenueName.Name, venue.VenueName.Name)
                    .SetProperty(v => v.VenueName.Prefix, venue.VenueName.Prefix)
                    .SetProperty(v => v.SeatsLimit, venue.SeatsLimit), cancellationToken);

        return venue.Id.Value;
    }

    public async Task<IReadOnlyList<Venue>> GetByPrefix(string prefix, CancellationToken cancellationToken = default)
    {
        var venues = await _dbContext.Venues
            .Where(v => v.VenueName.Prefix.StartsWith(prefix))
            .ToListAsync(cancellationToken);

        return venues;
    }

    public async Task<UnitResult<Error>> AddSeats(
        IEnumerable<Seat> seats, CancellationToken cancellationToken = default)
    {
        await _dbContext.Seats.AddRangeAsync(seats, cancellationToken);

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteSeatsByIdAsync(VenueId id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Seats
            .Where(s => s.Venue.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}