using CSharpFunctionalExtensions;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Contracts.Venues.UpdateVenue;
using Shared;

namespace ReservationService.Application.Venues.Commands.UpdateVenue;

public class UpdateVenueNameByPrefixHandler : ICommandHandler<UpdateVenueNameByPrefixRequest>
{
    private readonly IVenuesRepository _venueRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueNameByPrefixHandler(IVenuesRepository venueRepository, ITransactionManager transactionManager)
    {
        _venueRepository = venueRepository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Errors>> Handle(
        UpdateVenueNameByPrefixRequest request, CancellationToken cancellationToken)
    {
        var venues = await _venueRepository.GetByPrefix(request.Prefix, cancellationToken);

        foreach (var venue in venues)
        {
            venue.UpdateName(request.Name);
        }

        // var updateVenueResult = await _venueRepository.UpdateNameByPrefixAsync(
        //     request.Prefix, nameResult.Value, cancellationToken);
        // if (updateVenueResult.IsFailure)
        //     return updateVenueResult.Error.ToErrors();

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Errors>();
    }

    // public async Task<UnitResult<Errors>> Handle(
    //     UpdateVenueNameByPrefixRequest request, CancellationToken cancellationToken)
    // {
    //     // бизнес валидация
    //
    //     // доменная валидация
    //     var nameResult = VenueName.CreateWithPrefix(request.Name);
    //     if (nameResult.IsFailure)
    //         return nameResult.Error.ToErrors();
    //
    //     // обновление в бд
    //     var updateVenueResult = await _venueRepository.UpdateNameByPrefixAsync(
    //         request.Prefix, nameResult.Value, cancellationToken);
    //     if (updateVenueResult.IsFailure)
    //         return updateVenueResult.Error.ToErrors();
    //
    //     return UnitResult.Success<Errors>();
    // }
}