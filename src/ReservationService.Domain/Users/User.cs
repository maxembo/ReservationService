using CSharpFunctionalExtensions;
using ReservationService.Domain.Reservations;

namespace ReservationService.Domain.Users;

public class User
{
    private List<Reservation> _reservations = [];

    // ef core
    private User()
    {
    }

    private User(UserId id, Details details)
    {
        Id = id;
        Details = details;
    }

    public UserId Id { get; private set; }

    public Details Details { get; private set; }

    public IReadOnlyList<Reservation> Reservations => _reservations.AsReadOnly();

    public static Result<User> Create(UserId id, Details details)
    {
        var user = new User(id, details);

        return Result.Success(user);
    }

    public Result AddReservation(Reservation reservation)
    {
        _reservations.Add(reservation);
        
        return Result.Success();
    }
}