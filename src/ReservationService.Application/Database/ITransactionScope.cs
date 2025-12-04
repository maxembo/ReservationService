using CSharpFunctionalExtensions;
using Shared;

namespace ReservationService.Application.Database;

public interface ITransactionScope : IDisposable
{
    public UnitResult<Error> Commit();

    public UnitResult<Error> Rollback();
}