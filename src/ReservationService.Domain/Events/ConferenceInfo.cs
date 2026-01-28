namespace ReservationService.Domain.Events;

public record ConferenceInfo(string Speaker, string Topic) : IEventInfo
{
    public override string ToString() => $"Conference: {Speaker}|{Topic}";
}