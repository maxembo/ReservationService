using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Database;
using ReservationService.Contracts.Events;
using ReservationService.Contracts.Venues.Seats;
using ReservationService.Domain.Events;

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
                e => new GetEventDto
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
                    Seats = _readDbContext.SeatsRead
                        .Where(s => s.VenueId == e.VenueId)
                        .OrderBy(s => s.RowNumber)
                        .ThenBy(s => s.SeatNumber)
                        .Select(
                            s => new SeatDto
                            {
                                Id = s.Id.Value,
                                RowNumber = s.RowNumber,
                                SeatNumber = s.SeatNumber,
                                VenueId = s.VenueId.Value,
                            }).ToList(),
                })
            .FirstOrDefaultAsync(cancellationToken);
    }
}