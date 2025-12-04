using CSharpFunctionalExtensions;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Contracts.Venues.UpdateVenue;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Application.Venues.UpdateVenue;

public class UpdateVenueNameHandler : ICommandHandler<Guid, UpdateVenueNameRequest>
{
    private readonly IVenuesRepository _venueRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueNameHandler(IVenuesRepository venueRepository, ITransactionManager transactionManager)
    {
        _venueRepository = venueRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateVenueNameRequest request, CancellationToken cancellationToken)
    {
        var venueId = new VenueId(request.Id);
        var getVenueNameResult = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
        if (getVenueNameResult.IsFailure)
            return getVenueNameResult.Error.ToErrors();

        var nameResult = VenueName.CreateWithPrefix(request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error.ToErrors();

        var updateVenueNameResult =
            await _venueRepository.UpdateNameAsync(venueId, nameResult.Value, cancellationToken);
        if (updateVenueNameResult.IsFailure)
            return updateVenueNameResult.Error.ToErrors();

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return venueId.Value;
    }
}