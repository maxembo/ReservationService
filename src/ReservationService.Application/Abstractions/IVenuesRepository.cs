using CSharpFunctionalExtensions;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Application.Abstractions;

public interface IVenuesRepository
{
    Task<Result<Guid, Error>> AddAsync(Venue venue, CancellationToken cancellationToken = default);

    Task<Result<Guid, Error>> UpdateNameAsync(
        VenueId venueId, VenueName venueName, CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> UpdateNameByPrefixAsync(
        string prefix, VenueName name, CancellationToken cancellationToken = default);

    //Task SaveAsync(CancellationToken cancellationToken = default);

    Task<Result<Venue, Error>> GetByIdAsync(VenueId id, CancellationToken cancellationToken = default);

    Task<Result<Venue, Error>> GetSeatsByIdAsync(VenueId id, CancellationToken cancellationToken = default);

    public Task<Result<Guid, Error>> UpdateAsync(Venue venue, CancellationToken cancellationToken = default);

    public Task<IReadOnlyList<Venue>> GetByPrefix(string prefix, CancellationToken cancellationToken = default);

    //public Task<UnitResult<Error>> AddSeats(IEnumerable<Seat> seats, CancellationToken cancellationToken = default);

    public Task<UnitResult<Error>> DeleteSeatsByIdAsync(VenueId id, CancellationToken cancellationToken = default);
}