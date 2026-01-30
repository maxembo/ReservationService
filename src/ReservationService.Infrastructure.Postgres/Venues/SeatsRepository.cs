using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Venues;
using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Venues;
using ReservationService.Infrastructure.Postgres.Database;
using Shared;

namespace ReservationService.Infrastructure.Postgres.Venues;

public class SeatsRepository : ISeatsRepository
{
    private readonly ApplicationDbContext _context;

    public SeatsRepository(ApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<Seat>> GetByIdsAsync(
        IEnumerable<SeatId> seats, CancellationToken cancellationToken = default)
    {
        return await _context.Seats
            .Where(s => seats.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Seat>> GetAvailableSeatsAsync(
        VenueId venueId, EventId eventId, int? rowNumber, CancellationToken cancellationToken = default)
    {
        var seats = await _context.Seats
            .Where(s => s.VenueId == venueId)
            .Where(s => rowNumber.HasValue && s.RowNumber == rowNumber.Value)
            .Where(
                s => !_context.ReservationSeats.Any(
                    rs =>
                        rs.SeatId == s.Id &&
                         rs.EventId == eventId &&
                         (rs.Reservation.ReservationStatus == ReservationStatus.Confirmed ||
                        rs.Reservation.ReservationStatus == ReservationStatus.Pending)))
            .ToListAsync(cancellationToken);

        return seats;
    }
}