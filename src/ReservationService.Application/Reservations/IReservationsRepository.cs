using CSharpFunctionalExtensions;
using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Application.Reservations;

public interface IReservationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Reservation reservation, CancellationToken cancellationToken);

    Task<bool> AnySeatsAlreadyReserved(
        EventId eventId, IEnumerable<SeatId> seatIds, CancellationToken cancellationToken);
}