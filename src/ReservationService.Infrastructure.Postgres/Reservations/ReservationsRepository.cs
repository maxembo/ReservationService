using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Reservations;
using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Venues;
using ReservationService.Infrastructure.Postgres.Database;
using Shared;

namespace ReservationService.Infrastructure.Postgres.Reservations;

public class ReservationsRepository : IReservationsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReservationsRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<Guid, Error>> AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);

        var saveChangesResult = await _dbContext.SaveChangesResultAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error;

        return reservation.Id.Value;
    }

    public async Task<bool> AnySeatsAlreadyReserved(
        EventId eventId, IEnumerable<SeatId> seatIds, CancellationToken cancellationToken)
    {
        var hasSeatsReserved = await _dbContext.Reservations
            .Where(r => r.EventId == eventId.Value)
            .Where(r => r.ReservedSeats.Any(rs => seatIds.Contains(rs.SeatId)))
            .AnyAsync(cancellationToken);

        return hasSeatsReserved;
    }
}