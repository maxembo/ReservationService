using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Database;
using ReservationService.Contracts.Events;
using ReservationService.Contracts.Venues.Seats;
using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;

namespace ReservationService.Application.Events.Queries;

public class GetEventByIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetEventByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetEventDto?> Handle(GetEventByIdRequest query, CancellationToken cancellationToken)
    {
        return await _readDbContext.EventsRead
            .Include(e => e.Details)
            .Where(e => e.Id == new EventId(query.EventId))
            .Select(
                @event => new GetEventDto
                {
                    Id = @event.Id.Value,
                    VenueId = @event.VenueId.Value,
                    Name = @event.Name.Value,
                    Capacity = @event.Details.Capacity,
                    Description = @event.Details.Description,
                    Info = @event.Info.ToString(),
                    Status = @event.Status.ToString(),
                    LastReservationUtc = @event.Details.LastReservationUtc,
                    Type = @event.Type.ToString(),
                    StartDate = @event.StartDate,
                    EndDate = @event.EndDate,
                    EventDate = @event.EventDate,
                    Seats = (from s in _readDbContext.SeatsRead
                        where s.VenueId == @event.VenueId
                        join rs in _readDbContext.ReservationSeatsRead on
                            new { SeatId = s.Id, EventId = @event.Id, } equals new
                            {
                                SeatId = rs.SeatId, EventId = rs.EventId,
                            } into reservations
                        from r in reservations.DefaultIfEmpty()
                        where @event.Id == new EventId(query.EventId)
                        orderby s.RowNumber, s.SeatNumber
                        select new AvailableSeatDto()
                        {
                            Id = s.Id.Value,
                            RowNumber = s.RowNumber,
                            SeatNumber = s.SeatNumber,
                            VenueId = s.VenueId.Value,
                            IsAvailable = r == null,
                        }).ToList(),
                    TotalSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == @event.VenueId),
                    ReservedSeats = _readDbContext.ReservationSeatsRead.Count(
                        rs => rs.EventId == @event.Id &&
                              (rs.Reservation.ReservationStatus == ReservationStatus.Confirmed ||
                               rs.Reservation.ReservationStatus == ReservationStatus.Pending)),
                    AvailableSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == @event.VenueId) -
                                     _readDbContext.ReservationSeatsRead.Count(
                                         rs => rs.EventId == @event.Id &&
                                               (rs.Reservation.ReservationStatus == ReservationStatus.Confirmed ||
                                                rs.Reservation.ReservationStatus == ReservationStatus.Pending)),
                })
            .FirstOrDefaultAsync(cancellationToken);
    }
}