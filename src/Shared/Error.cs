namespace Shared;

public class Error
{
    public string Code { get; }

    public string Message { get; }

    public ErrorType ErrorType { get; }

    public string? InvalidField { get; }

    private Error(string code, string message, ErrorType errorType, string? invalidField = null)
    {
        Code = code;
        Message = message;
        ErrorType = errorType;
        InvalidField = invalidField;
    }

    public static Error Validation(string? code, string message, string? invalidField = null) =>
        new(code ?? "value.is.invalid", message, ErrorType.Validation, invalidField);

    public static Error NotFound(string? code, string message) =>
        new(code ?? "value.not.found", message, ErrorType.NotFound);

    public static Error Conflict(string? code, string message) =>
        new(code ?? "value.conflict", message, ErrorType.Conflict);

    public static Error Failure(string? code, string message) =>
        new(code ?? "value.failure", message, ErrorType.Failure);

    public Errors ToErrors() => new([this]);
}