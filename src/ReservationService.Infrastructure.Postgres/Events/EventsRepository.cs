using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Events;
using ReservationService.Domain.Events;
using ReservationService.Infrastructure.Postgres.Database;
using Shared;

namespace ReservationService.Infrastructure.Postgres.Events;

public class EventsRepository : IEventsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EventsRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<Event, Error>> GetByIdAsync(EventId id, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events
            .Include(e => e.Details)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (@event is null)
            return GeneralErrors.NotFound(id.Value, "eventId");

        return @event;
    }

    public async Task<Result<Event, Error>> GetByIdWithLockAsync(EventId id, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events
            .FromSql($"SELECT * FROM events WHERE id = {id.Value} FOR UPDATE")
            .Include(e => e.Details)
            .FirstOrDefaultAsync(cancellationToken);

        if (@event is null)
        {
            return GeneralErrors.NotFound(id.Value, "eventId");
        }

        return @event;
    }

    public async Task<Event?> GetById(EventId eventId, CancellationToken cancellationToken)
    {
        return await _dbContext.Events
            .Include(e => e.Details)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
    }
}