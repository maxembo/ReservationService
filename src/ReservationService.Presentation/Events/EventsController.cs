using Microsoft.AspNetCore.Mvc;
using ReservationService.Application.Events;
using ReservationService.Application.Events.Queries;
using ReservationService.Contracts.Events;

namespace ReservationService.Presentation.Events;

[ApiController]
[Route("/api/events")]
public class EventsController : ControllerBase
{
    [HttpGet("/{eventId:guid}")]
    public async Task<ActionResult<GetEventDto>> GetById(
        [FromRoute] Guid eventId,
        [FromServices] GetEventByIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var @event = await handler.Handle(new GetEventByIdRequest(eventId), cancellationToken);

        return Ok(@event);
    }

    [HttpGet("/{eventId:guid}/dapper")]
    public async Task<ActionResult<GetEventDto>> GetByIdDapper(
        [FromRoute] Guid eventId,
        [FromServices] GetEventByIdHandlerDapper handler,
        CancellationToken cancellationToken = default)
    {
        var @event = await handler.Handle(new GetEventByIdRequest(eventId), cancellationToken);

        return Ok(@event);
    }
}