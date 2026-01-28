using ReservationService.Contracts.Events;
using ReservationService.Domain.Events;

namespace ReservationService.Application.Events;

public class GetEventByIdHandler
{
    private readonly IEventsRepository _eventsRepository;

    public GetEventByIdHandler(IEventsRepository eventsRepository)
    {
        _eventsRepository = eventsRepository;
    }

    public async Task<GetEventDto?> Handle(GetEventByIdRequest request, CancellationToken cancellationToken)
    {
        var @event = await _eventsRepository.GetById(new EventId(request.EventId), cancellationToken);

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