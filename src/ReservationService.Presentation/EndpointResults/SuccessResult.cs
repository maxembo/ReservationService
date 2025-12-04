using System.Net;
using Microsoft.AspNetCore.Http;
using ReservationService.Presentation.Response;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace ReservationService.Presentation.EndpointResults;

public class SuccessResult<TValue> : IResult
{
    private readonly TValue _value;

    public SuccessResult(TValue value) => _value = value;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok(_value);

        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

        return httpContext.Response.WriteAsJsonAsync(envelope);
    }
}

public class SuccessResult : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok();

        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

        return httpContext.Response.WriteAsJsonAsync(envelope);
    }
}