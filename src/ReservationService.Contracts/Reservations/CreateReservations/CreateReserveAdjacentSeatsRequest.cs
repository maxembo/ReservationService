namespace ReservationService.Contracts.Reservations.CreateReservations;

public record CreateReserveAdjacentSeatsRequest(
    Guid EventId,
    Guid UserId,
    Guid VenueId,
    int RequiredSeatsCount,
    int? PreferredRowNumber);