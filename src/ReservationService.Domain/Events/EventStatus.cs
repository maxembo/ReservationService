namespace ReservationService.Domain.Events;

public enum EventStatus
{
    Planned,
    InProgress,
    Finished,
    Cancelled,
}