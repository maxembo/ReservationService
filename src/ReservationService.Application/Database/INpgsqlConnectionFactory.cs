using System.Data;

namespace ReservationService.Application.Database;

public interface INpgsqlConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}