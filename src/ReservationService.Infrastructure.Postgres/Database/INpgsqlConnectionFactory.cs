using System.Data;

namespace ReservationService.Infrastructure.Postgres.Database;

public interface INpgsqlConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}