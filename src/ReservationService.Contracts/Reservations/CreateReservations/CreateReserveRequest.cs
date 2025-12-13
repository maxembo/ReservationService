using Microsoft.Extensions.Logging;
using ReservationService.Domain.Users;

namespace ReservationService.Contracts.Reservations.CreateReservations;

public record CreateReserveRequest(Guid EventId, Guid UserId, IEnumerable<Guid> SeatIds);