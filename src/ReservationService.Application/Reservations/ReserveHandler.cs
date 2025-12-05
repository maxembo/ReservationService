using CSharpFunctionalExtensions;
using ReservationService.Application.Abstractions;
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

    public ReserveHandler(
        ISeatsRepository seatsRepository,
        IReservationsRepository reservationsRepository,
        IEventsRepository eventsRepository)
    {
        _seatsRepository = seatsRepository;
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
    }

    public async Task<Result<Guid, Errors>> Handle(CreateReserveRequest request, CancellationToken cancellationToken)
    {
        // 1. Валидация входные параметр

        // 2. Доступно мероприятие для бронирования. Проверить даты. Проверить статус.
        var eventId = new EventId(request.EventId);
        var userId = new UserId(request.UserId);

        var (_, isFailure, @event, error) = await _eventsRepository.GetByIdAsync(eventId, cancellationToken);
        if (isFailure)
            return error.ToErrors();

        if (@event.IsAvailableForReservation() == false)
        {
            return GeneralErrors.AlreadyExist("event").ToErrors();
        }

        // 3. Проверить что места принадлежат мероприятию и площадке
        var seatIds = request.SeatIds
            .Select(seatId => new SeatId(seatId))
            .ToList();

        var seats = await _seatsRepository.GetByIdsAsync(seatIds, cancellationToken);
        if (seats.Any(s => s.VenueId != @event.VenueId) || seatIds.Count == 0)
        {
            return GeneralErrors.AlreadyExist("event seats").ToErrors();
        }

        // 4. Проверить что места не забронированы на нужное мероприятие
        var isSeatsReserved =
            await _reservationsRepository.AnySeatsAlreadyReserved(eventId, seatIds, cancellationToken);
        if (isSeatsReserved)
        {
            return GeneralErrors.AlreadyExist("seats").ToErrors();
        }

        // Создать Reservation c ReservedSeats
        var reservationResult = Reservation.Create(request.EventId, userId, request.SeatIds);
        if (reservationResult.IsFailure)
            return reservationResult.Error.ToErrors();

        var reservation = reservationResult.Value;

        // Сохранить в базу данных
        var addReservationResult = await _reservationsRepository.AddAsync(reservation, cancellationToken);
        if (addReservationResult.IsFailure)
            return addReservationResult.Error.ToErrors();

        return reservation.Id.Value;
    }
}