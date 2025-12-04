using CSharpFunctionalExtensions;
using ReservationService.Domain.Shared;
using Shared;

namespace ReservationService.Domain.Venues;

public class Seat
{
    // ef core
    private Seat()
    {
    }

    private Seat(SeatId id, Venue venue, int rowNumber, int seatNumber)
    {
        Id = id;
        Venue = venue;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }

    private Seat(SeatId id, VenueId venueId, int rowNumber, int seatNumber)
    {
        Id = id;
        VenueId = venueId;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }

    public SeatId Id { get; } = null!;

    public int RowNumber { get; private set; }

    public int SeatNumber { get; private set; }

    public Venue Venue { get; private set; } = null!;

    public VenueId VenueId { get; private set; } = null!;

    public static Result<Seat, Error> Create(Venue venue, int rowNumber, int seatNumber)
    {
        if (rowNumber is < 0 or > Constants.MAX_ROW_NUMBER)
            return GeneralErrors.LengthOutOfRange("seat rowNumber", rowNumber, Constants.MAX_ROW_NUMBER);

        if (seatNumber is < 0 or > Constants.MAX_SEAT_NUMBER)
            return GeneralErrors.LengthOutOfRange("seat seatNumber", seatNumber, Constants.MAX_SEAT_NUMBER);

        return new Seat(new SeatId(Guid.NewGuid()), venue, rowNumber, seatNumber);
    }

    public static Result<Seat, Error> Create(VenueId venueId, int rowNumber, int seatNumber)
    {
        if (rowNumber is < 0 or > Constants.MAX_ROW_NUMBER)
            return GeneralErrors.LengthOutOfRange("seat rowNumber", Constants.MAX_ROW_NUMBER);

        if (seatNumber is < 0 or > Constants.MAX_SEAT_NUMBER)
            return GeneralErrors.LengthOutOfRange("seat seatNumber", Constants.MAX_SEAT_NUMBER);

        return new Seat(new SeatId(Guid.NewGuid()), venueId, rowNumber, seatNumber);
    }
}