using CSharpFunctionalExtensions;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Contracts.Venues.UpdateVenue;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Application.Venues.Commands.UpdateVenue;

public class UpdateVenueHandler : ICommandHandler<Guid, UpdateVenueRequest>
{
    private readonly IVenuesRepository _venueRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueHandler(IVenuesRepository venueRepository, ITransactionManager transactionManager)
    {
        _venueRepository = venueRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateVenueRequest request, CancellationToken cancellationToken)
    {
        var getVenueByIdResult = await _venueRepository.GetByIdAsync(new VenueId(request.Id), cancellationToken);
        if (getVenueByIdResult.IsFailure)
            return getVenueByIdResult.Error.ToErrors();

        //var nameResult = VenueName.CreateWithPrefix(request.Name);
        var nameResult = VenueName.Create(request.Prefix, request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error.ToErrors();

        var venueResult = Venue.Create(
            nameResult.Value.Prefix, nameResult.Value.Name, request.SeatLimits, getVenueByIdResult.Value.Id);

        if (venueResult.IsFailure)
            return venueResult.Error.ToErrors();

        //venueResult.Value.UpdateName(request.Name);

        await _venueRepository.UpdateAsync(venueResult.Value, cancellationToken);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        //await _venueRepository.SaveAsync(cancellationToken);

        return venueResult.Value.Id.Value;
    }

    // public async Task<Result<Guid, Errors>> Handle(UpdateVenueRequest request, CancellationToken cancellationToken)
    // {
    //     var getVenueByIdResult = await _venueRepository.GetByIdAsync(new VenueId(request.Id), cancellationToken);
    //     if (getVenueByIdResult.IsFailure)
    //         return getVenueByIdResult.Error.ToErrors();
    //
    //     //var nameResult = VenueName.CreateWithPrefix(request.Name);
    //     var nameResult = VenueName.Create(request.Prefix, request.Name);
    //     if (nameResult.IsFailure)
    //         return nameResult.Error.ToErrors();
    //
    //     var venueResult = Venue.Create(
    //         nameResult.Value.Prefix, nameResult.Value.Name, request.SeatLimits, getVenueByIdResult.Value.Id);
    //
    //     if (venueResult.IsFailure)
    //         return venueResult.Error.ToErrors();
    //
    //     //venueResult.Value.UpdateName(request.Name);
    //
    //     await _venueRepository.UpdateAsync(venueResult.Value, cancellationToken);
    //
    //     await _venueRepository.SaveAsync(cancellationToken);
    //
    //     return venueResult.Value.Id.Value;
    // }

    // public async Task<Result<Guid, Errors>> Handle(UpdateVenueRequest request, CancellationToken cancellationToken)
    // {
    //     // бизнес валидация
    //
    //     // доменная валидация
    //     var venueId = new VenueId(request.Id);
    //     var nameResult = VenueName.CreateWithPrefix(request.Name);
    //     if (nameResult.IsFailure)
    //         return nameResult.Error.ToErrors();
    //
    //     // обновление в бд
    //     var updateVenueResult = await _venueRepository.UpdateNameAsync(venueId, nameResult.Value, cancellationToken);
    //     if (updateVenueResult.IsFailure)
    //         return updateVenueResult.Error.ToErrors();
    //
    //     return venueId.Value;
    // }
}