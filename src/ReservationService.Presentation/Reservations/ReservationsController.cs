using Microsoft.AspNetCore.Mvc;
using ReservationService.Application.Abstractions;
using ReservationService.Contracts.Reservations.CreateReservations;
using ReservationService.Presentation.EndpointResults;

namespace ReservationService.Presentation.Reservations;

[ApiController]
[Route("/api/reservations")]
public class ReservationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateReserveRequest> handler,
        [FromBody] CreateReserveRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }

    [HttpPost("/adjacent")]
    public async Task<EndpointResult<Guid>> CreateAdjacentSeats(
        [FromServices] ICommandHandler<Guid, CreateReserveAdjacentSeatsRequest> handler,
        [FromBody] CreateReserveAdjacentSeatsRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }
}