using CSharpFunctionalExtensions;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Application.Events;
using ReservationService.Application.Venues;
using ReservationService.Contracts.Reservations.CreateReservations;
using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Users;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Application.Reservations.Commands;

public class ReserveAdjacentSeatsHandler : ICommandHandler<Guid, CreateReserveAdjacentSeatsRequest>
{
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventRepository;
    private readonly ISeatsRepository _seatsRepository;
    private readonly ITransactionManager _transactionManager;

    public ReserveAdjacentSeatsHandler(
        IReservationsRepository reservationsRepository,
        IEventsRepository eventRepository,
        ISeatsRepository seatsRepository,
        ITransactionManager transactionManager)
    {
        _reservationsRepository = reservationsRepository;
        _eventRepository = eventRepository;
        _seatsRepository = seatsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreateReserveAdjacentSeatsRequest request, CancellationToken cancellationToken)
    {
        var venueId = new VenueId(request.VenueId);
        var eventId = new EventId(request.EventId);
        var userId = new UserId(request.UserId);

        if (request.RequiredSeatsCount <= 0)
        {
            return GeneralErrors.Required("requiredSeatsCount").ToErrors();
        }

        if (request.RequiredSeatsCount > 10)
        {
            return GeneralErrors.Invalid("requiredSeatsCount").ToErrors();
        }

        var transactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            return transactionResult.Error.ToErrors();
        }

        using var transaction = transactionResult.Value;

        var (_, isFailure, @event, error) = await _eventRepository.GetByIdWithLockAsync(eventId, cancellationToken);
        if (isFailure)
        {
            transaction.Rollback();
            return error.ToErrors();
        }

        var availableSeats =
            await _seatsRepository.GetAvailableSeatsAsync(
                venueId, eventId, request.PreferredRowNumber, cancellationToken);
        if (availableSeats.Count == 0)
        {
            transaction.Rollback();
            return GeneralErrors.NotFound(null, "availableSeats").ToErrors();
        }

        var selectedSeats = request.PreferredRowNumber.HasValue
            ? AdjacentSeatsFinder.FindAdjacentSeatsInPreferredRow(
                availableSeats, request.RequiredSeatsCount, request.PreferredRowNumber.Value)
            : AdjacentSeatsFinder.FindBestAdjacentSeats(availableSeats, request.RequiredSeatsCount);
        if (selectedSeats.Count == 0)
        {
            transaction.Rollback();
            return GeneralErrors.NotFound(null, "selectedSeats").ToErrors();
        }

        if (selectedSeats.Count < request.RequiredSeatsCount)
        {
            transaction.Rollback();
            return GeneralErrors.Invalid("selectedSeats").ToErrors();
        }

        var seatIds = selectedSeats
            .Select(s => s.Id)
            .ToList();

        var reservationResult = Reservation.Create(eventId, userId, seatIds.Select(si => si.Value));
        if (reservationResult.IsFailure)
        {
            transaction.Rollback();
            return reservationResult.Error.ToErrors();
        }

        var reservation = reservationResult.Value;

        var addReservationResult = await _reservationsRepository.AddAsync(reservation, cancellationToken);
        if (addReservationResult.IsFailure)
        {
            transaction.Rollback();
            return addReservationResult.Error.ToErrors();
        }

        var commitedResult = transaction.Commit();
        if (commitedResult.IsFailure)
        {
            transaction.Rollback();
            return commitedResult.Error.ToErrors();
        }

        return reservation.Id.Value;
    }
}