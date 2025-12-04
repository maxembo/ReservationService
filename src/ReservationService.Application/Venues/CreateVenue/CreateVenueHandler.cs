using CSharpFunctionalExtensions;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Contracts.Venues.CreateVenue;
using ReservationService.Domain.Venues;
using Shared;

namespace ReservationService.Application.Venues.CreateVenue;

public class CreateVenueHandler : ICommandHandler<Guid, CreateVenueRequest>
{
    private readonly IVenuesRepository _venuesRepository;

    public CreateVenueHandler(IVenuesRepository venuesRepository)
    {
        _venuesRepository = venuesRepository;
    }

    /// <summary>
    /// Создает площадку со всеми местами.
    /// </summary>
    public async Task<Result<Guid, Errors>> Handle(
        CreateVenueRequest request, CancellationToken cancellationToken = default)
    {
        //валидация входных параметров

        //бизнес валидация

        //валидация доменных моделей
        // var seats = request.Seats
        //     .Select(s => Seat.Create(s.RowNumber, s.SeatNumber).Value)
        //     .ToList();

        var venueResult = Venue.Create(request.Prefix, request.Name, request.SeatLimits);
        if (venueResult.IsFailure)
            return venueResult.Error.ToErrors();

        List<Seat> seats = [];
        foreach (var seatRequest in request.Seats)
        {
            var seatResult = Seat.Create(venueResult.Value, seatRequest.RowNumber, seatRequest.SeatNumber);
            if (seatResult.IsFailure)
                return venueResult.Error.ToErrors();

            seats.Add(seatResult.Value);
        }

        venueResult.Value.AddSeats(seats);

        var addVenueResult = await _venuesRepository.AddAsync(venueResult.Value, cancellationToken);
        if (addVenueResult.IsFailure)
            return addVenueResult.Error.ToErrors();

        //сохранение доменных моделей в базу данных

        //await _dbContext.Venues.AddAsync(venue.Value, cancellationToken);

        //var entry = _dbContext.ChangeTracker.Entries();

        //await _dbContext.SaveChangesAsync(cancellationToken);

        return venueResult.Value.Id.Value;
    }
}