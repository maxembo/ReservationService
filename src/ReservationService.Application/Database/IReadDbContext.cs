using ReservationService.Domain.Events;

namespace ReservationService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Event> EventsRead { get; }
}