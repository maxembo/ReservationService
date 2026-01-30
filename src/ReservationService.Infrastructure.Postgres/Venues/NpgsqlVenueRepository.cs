using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using ReservationService.Application.Database;
using ReservationService.Application.Venues;
using ReservationService.Domain.Venues;
using ReservationService.Infrastructure.Postgres.Database;
using Shared;

namespace ReservationService.Infrastructure.Postgres.Venues;

public class NpgsqlVenueRepository : IVenuesRepository
{
    private readonly INpgsqlConnectionFactory _npgsqlConnectionFactory;
    private readonly ILogger<NpgsqlVenueRepository> _logger;

    public NpgsqlVenueRepository(
        INpgsqlConnectionFactory npgsqlConnectionFactory,
        ILogger<NpgsqlVenueRepository> logger)
    {
        _npgsqlConnectionFactory = npgsqlConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        using var connection = await _npgsqlConnectionFactory.CreateConnectionAsync(cancellationToken);

        var transaction = connection.BeginTransaction();

        try
        {
            const string venueInsertSql = """
                                          INSERT INTO venues (id, prefix, name, "SeatsLimit")
                                          Values (@Id, @Prefix, @Name, @SeatsLimit)
                                          """;

            var venueInsertParams = new
            {
                Id = venue.Id.Value,
                Prefix = venue.VenueName.Prefix,
                Name = venue.VenueName.Name,
                SeatsLimit = venue.SeatsLimit,
            };

            await connection.ExecuteAsync(venueInsertSql, venueInsertParams);

            if (!venue.Seats.Any())
            {
                return venue.Id.Value;
            }

            const string seatInsertSql = """
                                         INSERT INTO seats (id, row_number,seat_number,venue_id)
                                         VALUES (@Id, @RowNumber, @SeatNumber, @VenueId)
                                         """;

            var seatsInsertParams = venue.Seats.Select(
                seat => new
                {
                    Id = seat.Id.Value,
                    RowNumber = seat.RowNumber,
                    SeatNumber = seat.SeatNumber,
                    VenueId = venue.Id.Value,
                });

            await connection.ExecuteAsync(seatInsertSql, seatsInsertParams);

            transaction.Commit();

            return venue.Id.Value;
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            _logger.LogError("Failed to insert Venue.");

            return GeneralErrors.Database(ex.Message);
        }
    }

    public async Task<Result<Guid, Error>> UpdateNameAsync(
        VenueId venueId, VenueName venueName, CancellationToken cancellationToken = default)
    {
        using var connection = await _npgsqlConnectionFactory.CreateConnectionAsync(cancellationToken);

        var transaction = connection.BeginTransaction();

        try
        {
            const string venueUpdateSql = """
                                          UPDATE venues
                                          SET name = @Name
                                          WHERE id = @Id
                                          """;

            var venueUpdateParams = new { Id = venueId.Value, Name = venueName.Name, };

            await connection.ExecuteAsync(venueUpdateSql, venueUpdateParams);

            transaction.Commit();

            return venueId.Value;
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            _logger.LogError("Failed to update Venue.");

            return GeneralErrors.Database(ex.Message);
        }
    }

    public async Task<UnitResult<Error>> UpdateNameByPrefixAsync(
        string prefix, VenueName name, CancellationToken cancellationToken = default)
    {
        using var connection = await _npgsqlConnectionFactory.CreateConnectionAsync(cancellationToken);

        var transaction = connection.BeginTransaction();

        try
        {
            const string updateVenueNameByPrefixSql = """
                                                    UPDATE venues
                                                    SET name = @Name
                                                    WHERE prefix LIKE @Prefix
                                                  """;

            var updateVenueNameByPrefixParams = new { Prefix = $"{prefix}%", Name = name.Name, };

            await connection.ExecuteAsync(updateVenueNameByPrefixSql, updateVenueNameByPrefixParams);

            transaction.Commit();

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            _logger.LogError("Failed to update Venue.");

            return GeneralErrors.Database(ex.Message);
        }
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Venue, Error>> GetByIdAsync(VenueId id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Result<Venue, Error>> GetSeatsByIdAsync(VenueId id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Result<Guid, Error>> UpdateAsync(Venue venue, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<IReadOnlyList<Venue>> GetByPrefix(string prefix, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<UnitResult<Error>> AddSeats(IEnumerable<Seat> seats, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<UnitResult<Error>> DeleteSeatsByIdAsync(VenueId id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}