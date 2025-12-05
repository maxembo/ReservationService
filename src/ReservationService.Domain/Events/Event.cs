using CSharpFunctionalExtensions;
using ReservationService.Domain.Venues;
using Shared;

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
    {
    }

    private Event(
        EventId id,
        VenueId venueId,
        EventDetails details,
        EventName name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
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
        StartDate = startDate;
        EndDate = endDate;
        Status = EventStatus.Planned;
    }

    public EventId Id { get; }

    public VenueId VenueId { get; private set; }

    public EventDetails Details { get; private set; }

    public EventName Name { get; private set; }

    public EventType Type { get; private set; }

    public IEventInfo Info { get; private set; }

    public DateTime EventDate { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public EventStatus Status { get; private set; }

    public bool IsAvailableForReservation() =>
        Status != EventStatus.Planned || StartDate >= DateTime.UtcNow;

    public static Result<Event, Error> Create(
        VenueId venueId,
        EventDetails details,
        EventName name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        IEventInfo info,
        EventType type)
    {
        if (startDate > endDate || startDate <= DateTime.UtcNow || endDate <= DateTime.UtcNow)
        {
            return GeneralErrors.Invalid("startDate or endDate");
        }

        return new Event(
            new EventId(Guid.NewGuid()), venueId, details, name, eventDate, startDate, endDate, info, type);
    }

    public static Result<Event, Error> CreateConcert(
        VenueId venueId,
        EventDetails details,
        EventName name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        string performer)
    {
        if (string.IsNullOrWhiteSpace(performer))
            return GeneralErrors.Required("performer");

        var concertInfo = new ConcertInfo(performer);

        return new Event(
            new EventId(Guid.NewGuid()), venueId, details, name, eventDate, startDate, endDate, concertInfo,
            EventType.Concert);
    }

    public static Result<Event, Error> CreateOnline(
        VenueId venueId,
        EventDetails details,
        EventName name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return GeneralErrors.Required("url");

        var onlineInfo = new OnlineInfo(url);

        return new Event(
            new EventId(Guid.NewGuid()), venueId, details, name, eventDate, startDate, endDate, onlineInfo,
            EventType.Online);
    }

    public static Result<Event, Error> CreateConference(
        VenueId venueId,
        EventDetails details,
        EventName name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        string speaker,
        string topic)
    {
        if (string.IsNullOrWhiteSpace(speaker))
            return GeneralErrors.Required("speaker");

        if (string.IsNullOrWhiteSpace(topic))
            return GeneralErrors.Required("topic");

        var conferenceInfo = new ConferenceInfo(speaker, topic);
        return new Event(
            new EventId(Guid.NewGuid()), venueId, details, name, eventDate, startDate, endDate, conferenceInfo,
            EventType.Conference);
    }
}