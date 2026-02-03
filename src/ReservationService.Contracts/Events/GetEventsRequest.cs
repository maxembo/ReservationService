namespace ReservationService.Contracts.Events;

public record GetEventsRequest(
    string? Search,
    string? EventType,
    DateTime? DateFrom,
    DateTime? DateTo,
    string? Status,
    Guid? VenueId,
    int? MinAvailableSeats,
    PaginationRequest Pagination,
    string? SortBy,
    string? SortDirection);