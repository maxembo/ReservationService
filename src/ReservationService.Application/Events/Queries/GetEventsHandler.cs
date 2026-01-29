using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Database;
using ReservationService.Contracts.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Venues;

namespace ReservationService.Application.Events.Queries;

public class GetEventsHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetEventsHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetEventsDto> Handle(GetEventsRequest query, CancellationToken cancellationToken)
    {
        var eventsQuery = _readDbContext.EventsRead;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            eventsQuery = eventsQuery.Where(
                e => EF.Functions.Like(e.Name.Value.ToLower(), $"%{query.Search.ToLower()}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.EventType))
            eventsQuery = eventsQuery.Where(e => e.Type.ToString().ToLower() == query.EventType.ToLower());

        if (query.DateFrom.HasValue)
            eventsQuery = eventsQuery.Where(e => e.EventDate >= query.DateFrom.Value.ToUniversalTime());

        if (query.DateTo.HasValue)
            eventsQuery = eventsQuery.Where(e => e.EventDate <= query.DateTo.Value.ToUniversalTime());

        if (!string.IsNullOrWhiteSpace(query.Status))
            eventsQuery = eventsQuery.Where(e => e.Status.ToString().ToLower() == query.Status.ToLower());

        if (query.VenueId.HasValue)
            eventsQuery = eventsQuery.Where(e => e.VenueId == new VenueId(query.VenueId.Value));

        if (query.MinAvailableSeats.HasValue)
        {
            eventsQuery = eventsQuery.Where(
                e => _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) -
                     _readDbContext.ReservationsRead.Count(
                         rs => rs.EventId == e.Id && (rs.ReservationStatus == ReservationStatus.Confirmed ||
                                                      rs.ReservationStatus == ReservationStatus.Pending)) >=
                     query.MinAvailableSeats.Value);
        }

        var totalCount = await eventsQuery.LongCountAsync(cancellationToken);

        eventsQuery = eventsQuery
            .OrderBy(e => e.EventDate)
            .Skip((query.Pagination.Page - 1) * query.Pagination.PageSize)
            .Take(query.Pagination.PageSize);

        var events = await eventsQuery
            .Select(
                e => new EventDto
                {
                    Id = e.Id.Value,
                    VenueId = e.VenueId.Value,
                    Name = e.Name.Value,
                    Capacity = e.Details.Capacity,
                    Description = e.Details.Description,
                    Info = e.Info.ToString(),
                    Status = e.Status.ToString(),
                    LastReservationUtc = e.Details.LastReservationUtc,
                    Type = e.Type.ToString(),
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    EventDate = e.EventDate,
                    TotalSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId),
                    ReservedSeats = _readDbContext.ReservationSeatsRead.Count(
                        rs => rs.EventId == e.Id && (rs.Reservation.ReservationStatus == ReservationStatus.Confirmed ||
                                                     rs.Reservation.ReservationStatus == ReservationStatus.Pending)),
                    AvailableSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) -
                                     _readDbContext.ReservationSeatsRead.Count(
                                         rs => rs.EventId == e.Id &&
                                               (rs.Reservation.ReservationStatus == ReservationStatus.Confirmed ||
                                                rs.Reservation.ReservationStatus == ReservationStatus.Pending)),
                })
            .ToListAsync(cancellationToken);

        return new GetEventsDto(events, totalCount);
    }
}