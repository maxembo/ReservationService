namespace ReservationService.Contracts.Events;

public record GetEventsDto(List<EventDto> EventDto, long TotalCount);

public record EventDto
{
    public Guid Id { get; init; }

    public Guid VenueId { get; init; }

    public string Description { get; init; } = string.Empty;

    public int Capacity { get; init; }

    public DateTime? LastReservationUtc { get; init; }

    public string Name { get; init; } = null!;

    public string Type { get; init; } = string.Empty;

    public string Info { get; init; } = string.Empty;

    public DateTime EventDate { get; init; }

    public DateTime StartDate { get; init; }

    public DateTime EndDate { get; init; }

    public string Status { get; init; } = null!;

    public int TotalSeats { get; init; }

    public int AvailableSeats { get; init; }

    public int ReservedSeats { get; init; }
}