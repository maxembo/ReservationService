using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Users;
using ReservationService.Domain.Venues;

namespace ReservationService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Event> EventsRead { get; }

    IQueryable<Reservation> ReservationsRead { get; }

    IQueryable<Seat> SeatsRead { get; }

    IQueryable<Venue> VenuesRead { get; }

    IQueryable<ReservationSeat> ReservationSeatsRead { get; }

    IQueryable<User> UsersRead { get; }
}