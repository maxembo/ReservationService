using Microsoft.AspNetCore.Mvc;
using ReservationService.Application.Abstractions;
using ReservationService.Application.Database;
using ReservationService.Contracts.Venues.CreateVenue;
using ReservationService.Contracts.Venues.UpdateVenue;
using ReservationService.Presentation.EndpointResults;
using ReservationService.Presentation.Response;

namespace ReservationService.Presentation.Venues;

[ApiController]
[Route("/api/venues")]
public class VenuesController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(500)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(403)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(401)]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateVenueRequest> handler,
        [FromBody] CreateVenueRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }

    [HttpPatch("/name")]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(500)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(403)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(401)]
    public async Task<EndpointResult<Guid>> UpdateVenueName(
        [FromServices] ICommandHandler<Guid, UpdateVenueNameRequest> handler,
        [FromBody] UpdateVenueNameRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }

    [HttpPatch("/name/by-prefix")]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(500)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(403)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(401)]
    public async Task<EndpointResult<Guid>> UpdateVenueNameByPrefix(
        [FromServices] ICommandHandler<UpdateVenueNameByPrefixRequest> handler,
        [FromBody] UpdateVenueNameByPrefixRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }

    [HttpPut]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(500)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(403)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(401)]
    public async Task<EndpointResult<Guid>> UpdateVenueAsync(
        [FromServices] ICommandHandler<Guid, UpdateVenueRequest> handler,
        [FromBody] UpdateVenueRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }

    [HttpPatch("/seats")]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(500)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    [ProducesResponseType<Envelope>(403)]
    [ProducesResponseType<Envelope>(404)]
    [ProducesResponseType<Envelope>(401)]
    public async Task<EndpointResult<Guid>> UpdateSeatsAsync(
        [FromServices] ICommandHandler<Guid, UpdateVenueSeatsRequest> handler,
        [FromBody] UpdateVenueSeatsRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }
}