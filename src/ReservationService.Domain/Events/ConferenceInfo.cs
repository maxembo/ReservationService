namespace ReservationService.Domain.Events;

public record ConferenceInfo(string Speaker, string Topic) : IEventInfo;