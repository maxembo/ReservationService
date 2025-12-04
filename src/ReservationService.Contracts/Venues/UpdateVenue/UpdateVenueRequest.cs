namespace ReservationService.Contracts.Venues.UpdateVenue;

public record UpdateVenueRequest(Guid Id, string Name, string Prefix, int SeatLimits);

//public record UpdateVenueRequest(Guid Id, string Name);