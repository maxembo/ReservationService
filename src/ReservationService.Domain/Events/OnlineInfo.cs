namespace ReservationService.Domain.Events;

public record OnlineInfo(string Url) : IEventInfo;