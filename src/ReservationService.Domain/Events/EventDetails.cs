using CSharpFunctionalExtensions;
using ReservationService.Domain.Shared;

namespace ReservationService.Domain.Events;

public class EventDetails
{
    // ef core
    private EventDetails()
    { }
    
    private EventDetails(string description, int capacity)
    {
        Description = description;
        Capacity = capacity;
    }

    public EventId EventId { get; }

    public string Description { get; private set; }

    public int Capacity { get; private set; }

    public static Result<EventDetails> Create(string description, int capacity)
    {
        if (string.IsNullOrWhiteSpace(description) ||
            description.Length > Constants.MAX_EVENT_DETAILS_DESCRIPTION_LENGTH)
            return Result.Failure<EventDetails>("EventDetails description невалидный");

        if (capacity <= 0 || capacity >= Constants.MAX_EVENT_DETAILS_CAPACITY)
            return Result.Failure<EventDetails>("EventDetails capacity невалидный");

        var eventDetails = new EventDetails(description, capacity);

        return Result.Success(eventDetails);
    }
}