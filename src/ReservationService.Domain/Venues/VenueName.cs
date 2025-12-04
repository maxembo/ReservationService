using CSharpFunctionalExtensions;
using ReservationService.Domain.Shared;
using Shared;

namespace ReservationService.Domain.Venues;

public record VenueName
{
    private VenueName(string prefix, string name)
    {
        Prefix = prefix;
        Name = name;
    }

    public string Name { get; }

    public string Prefix { get; }

    public override string ToString() => $"{Prefix}-{Name}";

    public static Result<VenueName, Error> CreateWithPrefix(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return GeneralErrors.Required("venueName");

        if (name.Length > Constants.MAX_NAME_LENGTH)
            return GeneralErrors.LengthOutOfRange("venue name", Constants.MAX_NAME_LENGTH);

        var venueName = new VenueName(string.Empty, name);

        return new VenueName(string.Empty, name);
    }

    public static Result<VenueName, Error> Create(string prefix, string name)
    {
        if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(name))
            return GeneralErrors.Required("venueName");

        if (prefix.Length > Constants.MAX_NAME_LENGTH || name.Length > Constants.MAX_NAME_LENGTH)
            return GeneralErrors.LengthOutOfRange("venue name", Constants.MAX_NAME_LENGTH);

        return new VenueName(prefix, name);
    }
}