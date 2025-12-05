using ReservationService.Domain.Venues;

namespace ReservationService.Application.Venues;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIdsAsync(IEnumerable<SeatId> seats, CancellationToken cancellationToken = default);
}