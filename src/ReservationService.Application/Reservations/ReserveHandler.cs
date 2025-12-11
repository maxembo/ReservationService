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

namespace ReservationService.Application.Reservations;

public class ReserveHandler : ICommandHandler<Guid, CreateReserveRequest>
{
    private readonly ISeatsRepository _seatsRepository;
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly ITransactionManager _transactionManager;

    public ReserveHandler(
        ISeatsRepository seatsRepository,
        IReservationsRepository reservationsRepository,
        IEventsRepository eventsRepository,
        ITransactionManager transactionManager)
    {
        _seatsRepository = seatsRepository;
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(CreateReserveRequest request, CancellationToken cancellationToken)
    {
        // 1. Валидация входные параметр

        // 2. Доступно мероприятие для бронирования. Проверить даты. Проверить статус.
        var eventId = new EventId(request.EventId);
        var userId = new UserId(request.UserId);

        var transactionResult =
            await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            return transactionResult.Error.ToErrors();
        }

        using var transaction = transactionResult.Value;

        var (_, isFailure, @event, error) = await _eventsRepository.GetByIdAsync(eventId, cancellationToken);
        if (isFailure)
            return error.ToErrors();

        int reservedSeatCount = await _reservationsRepository.GetReservedSeatsCount(@eventId, cancellationToken);

        if (@event.IsAvailableForReservation(reservedSeatCount + request.SeatIds.Count()) == false)
        {
            transaction.Rollback();
            return GeneralErrors.AlreadyExist("event").ToErrors();
        }

        // 3. Проверить что места принадлежат мероприятию и площадке
        var seatIds = request.SeatIds
            .Select(seatId => new SeatId(seatId))
            .ToList();

        var seats = await _seatsRepository.GetByIdsAsync(seatIds, cancellationToken);
        if (seats.Any(s => s.VenueId != @event.VenueId) || seatIds.Count == 0)
        {
            transaction.Rollback();
            return GeneralErrors.AlreadyExist("event seats").ToErrors();
        }

        // Создать Reservation c ReservedSeats
        var reservationResult = Reservation.Create(request.EventId, userId, request.SeatIds);
        if (reservationResult.IsFailure)
        {
            transaction.Rollback();
            return reservationResult.Error.ToErrors();
        }

        var reservation = reservationResult.Value;

        // Сохранить в базу данных
        var addReservationResult = await _reservationsRepository.AddAsync(reservation, cancellationToken);
        if (addReservationResult.IsFailure)
        {
            transaction.Rollback();
            return addReservationResult.Error.ToErrors();
        }

        @event.Details.ReserveSeat();

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            transaction.Rollback();
            return saveChangesResult.Error.ToErrors();
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