using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using ReservationService.Application.Database;

namespace ReservationService.Infrastructure.Postgres.Database;

public class NpgsqlConnectionFactory : IDisposable, IAsyncDisposable, INpgsqlConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        var dataSourceBuilder
            = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("ReservationServiceDb"));
        dataSourceBuilder
            .UseLoggerFactory(CreateLoggerFactory());

        _dataSource = dataSourceBuilder.Build();
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
        => await _dataSource.OpenConnectionAsync(cancellationToken);

    private ILoggerFactory CreateLoggerFactory()
        => LoggerFactory.Create(configure => configure.AddConsole());

    public void Dispose() => _dataSource.Dispose();

    public async ValueTask DisposeAsync() => await _dataSource.DisposeAsync();
}