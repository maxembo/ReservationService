using CSharpFunctionalExtensions;
using ReservationService.Domain.Events;
using ReservationService.Domain.Venues;

namespace ReservationService.Domain.Reservations;

public class ReservationSeat
{
    // ef core
    private ReservationSeat()
    { }

    private ReservationSeat(ReservationSeatId id, Reservation reservation, SeatId seatId, Guid eventId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        EventId = eventId;
    }

    public ReservationSeatId Id { get; }

    public Reservation Reservation { get; private set; }

    public ReservationId ReservationId { get; private set; }

    public SeatId SeatId { get; private set; }

    public Guid EventId { get; private set; }

    public DateTime ReservedAt { get; private set; } = DateTime.UtcNow;

    public static Result<ReservationSeat> Create(
        ReservationSeatId id, Reservation reservation, SeatId seatId, Guid eventId)
    {
        var reservationSeat = new ReservationSeat(id, reservation, seatId, eventId);

        return Result.Success(reservationSeat);
    }
}