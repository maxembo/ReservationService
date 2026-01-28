using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Database;
using ReservationService.Contracts.Events;
using ReservationService.Domain.Events;

namespace ReservationService.Application.Events.Queries;

public class GetEventByIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetEventByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetEventDto?> Handle(GetEventByIdRequest query, CancellationToken cancellationToken)
    {
        var @event = await _readDbContext.EventsRead
            .Include(e => e.Details)
            .FirstOrDefaultAsync(e => e.Id == new EventId(query.EventId), cancellationToken);

        if (@event is null)
        {
            return null;
        }

        return new GetEventDto()
        {
            Id = @event.Id.Value,
            VenueId = @event.VenueId.Value,
            Name = @event.Name.Value,
            Capacity = @event.Details.Capacity,
            Description = @event.Details.Description,
            Info = @event.Info.ToString(),
            Status = @event.Status.ToString(),
            LastReservationUtc = @event.Details.LastReservationUtc,
            Type = @event.Type.ToString(),
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            EventDate = @event.EventDate,
        };
    }
}