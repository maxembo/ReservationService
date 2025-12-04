using ReservationService.Domain.Venues;

namespace ReservationService.Contracts.Venues.CreateVenue;

public record CreateVenueDto(VenueId Id, VenueName Name);
