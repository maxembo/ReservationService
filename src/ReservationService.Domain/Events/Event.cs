using CSharpFunctionalExtensions;
using ReservationService.Domain.Venues;

namespace ReservationService.Domain.Events;

public class Event
{
    // Есть 3 способа как связать с Venue:
    // 1. сделать это в Venue списком(как с Seats);
    // 2. в Event сделать с помощью id VenueId;
    // 3. создать отдельный класс например VenueDetails, который будет объединять Event и EventDetails,
    // в базе данных это будет одна и та же сущность, в коде 2 сущности.

    // ef core
    private Event()
    { }

    private Event(
        EventId id,
        VenueId venueId,
        EventDetails details,
        EventName name,
        DateTime eventDate,
        IEventInfo info,
        EventType type)
    {
        Id = id;
        Name = name;
        VenueId = venueId;
        Details = details;
        EventDate = eventDate;
        Info = info;
        Type = type;
    }

    public EventId Id { get; }

    public VenueId VenueId { get; private set; }

    public EventDetails Details { get; private set; }

    public EventName Name { get; private set; }

    public EventType Type { get; private set; }

    public IEventInfo Info { get; private set; }

    public DateTime EventDate { get; private set; }

    public static Result<Event> Create(
        EventId id,
        VenueId venueId,
        EventDetails details,
        EventName name,
        DateTime eventDate,
        IEventInfo info,
        EventType type)
    {
        var @event = new Event(id, venueId, details, name, eventDate, info, type);

        return Result.Success(@event);
    }
}