using CSharpFunctionalExtensions;
using ReservationService.Domain.Events;
using Shared;

namespace ReservationService.Application.Events;

public interface IEventsRepository
{
    Task<Result<Event, Error>> GetByIdAsync(EventId id, CancellationToken cancellationToken);
}