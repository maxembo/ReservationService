using Shared;

namespace ReservationService.Presentation.Response;

public class Envelope
{
    public object? Result { get; }

    public Errors? ErrorList { get; }

    public bool IsError => ErrorList != null || (ErrorList != null && ErrorList.Any());

    public DateTime TimeGenerated { get; }

    public Envelope(object? result, Errors? errorList)
    {
        Result = result;
        ErrorList = errorList;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result = null) => new(result, null);

    public static Envelope Error(Errors errorList) => new(null, errorList);
}

public class Envelope<T>
{
    public T? Result { get; }

    public Errors? ErrorList { get; }

    public bool IsError => ErrorList != null || (ErrorList != null && ErrorList.Any());

    public DateTime TimeGenerated { get; }

    public Envelope(T? result, Errors? errorList)
    {
        Result = result;
        ErrorList = errorList;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope<T> Ok(T? result = default) => new(result, null);

    public static Envelope<T> Error(Errors errorList) => new(default, errorList);
}