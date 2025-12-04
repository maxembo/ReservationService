using ReservationService.Domain.Venues;

namespace ReservationService.Contracts.Venues.UpdateVenue;

public record UpdateVenueSeatsRequest(Guid VenueId, IEnumerable<UpdateSeatsRequest> Seats);