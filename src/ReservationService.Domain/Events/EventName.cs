using CSharpFunctionalExtensions;
using ReservationService.Domain.Shared;

namespace ReservationService.Domain.Events;

public record EventName
{
    private EventName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<EventName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_NAME_LENGTH)
            return Result.Failure<EventName>($"EventName не должен быть пустым или больше{Constants.MAX_NAME_LENGTH}");

        var eventName = new EventName(value);

        return Result.Success(eventName);
    }
}