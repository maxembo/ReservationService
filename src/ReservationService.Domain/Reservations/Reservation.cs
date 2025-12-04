using CSharpFunctionalExtensions;
using ReservationService.Domain.Users;
using ReservationService.Domain.Venues;

namespace ReservationService.Domain.Reservations;

public class Reservation
{
    // Два варианта создания связи многим ко многим между Reservation и Seats:
    // 1. Создать в Reservation список Seats, а в Seats список Reservation. Но у них прямая ссылка, что не очень хорошо;
    // 2. Создать класс ReservationSeat.

    private readonly List<ReservationSeat> _reservedSeats;

    // ef core
    private Reservation()
    { }

    private Reservation(ReservationId id, Guid eventId, UserId userId, IEnumerable<Guid> seatsIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;

        var reservedSeats = seatsIds
            .Select(
                seatId => ReservationSeat.Create(new ReservationSeatId(Guid.NewGuid()), this, new SeatId(seatId)).Value)
            .ToList();

        _reservedSeats = reservedSeats;
    }

    public ReservationId Id { get; }

    public Guid SeatId { get; private set; }

    public Guid EventId { get; private set; }

    public UserId UserId { get; private set; }

    public ReservationStatus ReservationStatus { get; private set; } = ReservationStatus.Pending;

    public IReadOnlyList<ReservationSeat> ReservedSeats => _reservedSeats;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public static Result<Reservation> Create(ReservationId id, Guid eventId, UserId userId, IEnumerable<Guid> seatsIds)
    {
        var reservation = new Reservation(id, eventId, userId, seatsIds);

        return Result.Success(reservation);
    }
}