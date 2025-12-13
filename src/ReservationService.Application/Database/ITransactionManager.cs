using System.Data;
using CSharpFunctionalExtensions;
using Shared;

namespace ReservationService.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken cancellationToken, IsolationLevel? level = null);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}