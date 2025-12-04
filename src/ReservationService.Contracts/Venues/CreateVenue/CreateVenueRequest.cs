namespace ReservationService.Contracts.Venues.CreateVenue;

public record CreateVenueRequest(
    string Prefix,
    string Name,
    int SeatLimits,
    IEnumerable<CreateSeatRequest> Seats);