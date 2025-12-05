using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Venues;
using ReservationService.Domain.Venues;
using ReservationService.Infrastructure.Postgres.Database;

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
}