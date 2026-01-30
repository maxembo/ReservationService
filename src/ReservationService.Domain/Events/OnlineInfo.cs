namespace ReservationService.Domain.Events;

public record OnlineInfo(string Url) : IEventInfo
{
    public override string ToString() => $"Online: {Url}";
}