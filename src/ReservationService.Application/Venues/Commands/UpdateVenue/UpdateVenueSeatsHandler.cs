using CSharpFunctionalExtensions;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Contracts.Venues.UpdateVenue;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Application.Venues.Commands.UpdateVenue;

public class UpdateVenueSeatsHandler : ICommandHandler<Guid, UpdateVenueSeatsRequest>
{
    private readonly IVenuesRepository _venueRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueSeatsHandler(IVenuesRepository venueRepository, ITransactionManager transactionManager)
    {
        _venueRepository = venueRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateVenueSeatsRequest request, CancellationToken cancellationToken)
    {
        var venueId = new VenueId(request.VenueId);

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transaction = transactionScopeResult.Value;

        var getVenueSeatsResult = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
        if (getVenueSeatsResult.IsFailure)
        {
            transaction.Rollback();
            return getVenueSeatsResult.Error.ToErrors();
        }

        var venue = getVenueSeatsResult.Value;

        List<Seat> seats = [];
        foreach (var seatRequest in request.Seats)
        {
            var seatResult = Seat.Create(venueId, seatRequest.RowNumber, seatRequest.SeatNumber);
            if (seatResult.IsFailure)
            {
                transaction.Rollback();
                return seatResult.Error.ToErrors();
            }

            seats.Add(seatResult.Value);
        }

        venue.UpdateSeats(seats);

        await _venueRepository.DeleteSeatsByIdAsync(venueId, cancellationToken);

        //await _venueRepository.AddSeats(seats, cancellationToken);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        var commitedResult = transaction.Commit();
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();

        return venueId.Value;
    }

    // public async Task<Result<Guid, Errors>> Handle(UpdateVenueSeatsRequest request, CancellationToken cancellationToken)
    // {
    //     var venueId = new VenueId(request.VenueId);
    //
    //     //var (_, error, isFailure, venue) = await _venueRepository.GetSeatsByIdAsync(venueId, cancellationToken);
    //     var getVenueSeatsResult = await _venueRepository.GetSeatsByIdAsync(venueId, cancellationToken);
    //     if (getVenueSeatsResult.IsFailure)
    //         return getVenueSeatsResult.Error.ToErrors();
    //
    //     var venue = getVenueSeatsResult.Value;
    //
    //     //var seats = venue.Seats.Select(s => Seat.Create(venue, s.RowNumber, s.RowNumber)).ToList();
    //
    //     List<Seat> seats = [];
    //     foreach (var seatRequest in request.Seats)
    //     {
    //         var seatResult = Seat.Create(venue, seatRequest.RowNumber, seatRequest.SeatNumber);
    //         if (seatResult.IsFailure)
    //             return seatResult.Error.ToErrors();
    //
    //         seats.Add(seatResult.Value);
    //     }
    //
    //     venue.UpdateSeats(seats);
    //
    //     await _venueRepository.SaveAsync(cancellationToken);
    //
    //     return venueId.Value;
    // }
}