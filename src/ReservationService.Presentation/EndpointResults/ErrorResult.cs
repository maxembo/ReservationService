using Microsoft.AspNetCore.Http;
using ReservationService.Presentation.Response;
using Shared;

namespace ReservationService.Presentation.EndpointResults;

public class ErrorResult : IResult
{
    private readonly Errors _errors;

    public ErrorResult(Errors errors) => _errors = errors;

    public ErrorResult(Error error) => _errors = error.ToErrors();

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Error(_errors);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return httpContext.Response.WriteAsJsonAsync(envelope);
    }
}