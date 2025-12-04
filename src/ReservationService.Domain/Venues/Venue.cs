using CSharpFunctionalExtensions;
using Shared;

namespace ReservationService.Domain.Venues;

public class Venue
{
    // ef core
    private Venue()
    {
    }

    private Venue(VenueId venueId, VenueName name, int seatsLimit)
    {
        Id = venueId;
        VenueName = name;
        SeatsLimit = seatsLimit;
    }

    private List<Seat> _seats = [];

    public VenueId Id { get; } = null!;

    public VenueName VenueName { get; private set; } = null!;

    public IReadOnlyList<Seat> Seats => _seats;

    public int SeatsLimit { get; private set; }

    public int SeatsCount => _seats.Count;

    public static Result<Venue, Error> Create(string prefix, string name, int seatsLimit, VenueId? venueId = null)
    {
        if (seatsLimit <= 0)
            return GeneralErrors.Invalid("venue seatsLimit");

        var venueNameResult = VenueName.Create(prefix, name);
        if (venueNameResult.IsFailure)
            return GeneralErrors.Invalid("venue venueName");

        var venue = new Venue(venueId ?? new VenueId(Guid.NewGuid()), venueNameResult.Value, seatsLimit);

        return venue;
    }

    public UnitResult<Error> UpdateSeats(IEnumerable<Seat> seats)
    {
        var seatsList = seats.ToList();

        if (seatsList.Count > SeatsLimit)
            return GeneralErrors.Invalid("seats limit");

        _seats = seatsList.ToList();
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateName(string name)
    {
        var nameResult = VenueName.Create(VenueName.Prefix, name);
        if (nameResult.IsFailure)
            return GeneralErrors.Invalid("venue name");

        VenueName = nameResult.Value;

        return UnitResult.Success<Error>();
    }

    public Result AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
            return Result.Failure("Seats limit is exceeded.");

        _seats.Add(seat);

        return Result.Success();
    }

    public void AddSeats(IEnumerable<Seat> seats) => _seats.AddRange(seats);

    public void ExpandSeatsLimit(int newSeatsLimit) => SeatsLimit = newSeatsLimit;
}