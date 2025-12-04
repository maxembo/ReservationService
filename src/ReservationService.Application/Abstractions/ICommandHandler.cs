using CSharpFunctionalExtensions;
using Shared;

namespace ReservationService.Application.Abstractions;

public interface ICommandHandler<TValue, in TCommand>
{
    Task<Result<TValue, Errors>> Handle(TCommand request, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand>
{
    Task<UnitResult<Errors>> Handle(TCommand request, CancellationToken cancellationToken);
}