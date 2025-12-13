using ReservationService.Domain.Events;
using ReservationService.Domain.Venues;

namespace ReservationService.Application.Venues;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIdsAsync(IEnumerable<SeatId> seats, CancellationToken cancellationToken = default);

    public Task<IReadOnlyList<Seat>> GetAvailableSeatsAsync(
        VenueId venueId, EventId eventId, int? rowNumber, CancellationToken cancellationToken = default);
}