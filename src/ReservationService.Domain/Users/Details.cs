using CSharpFunctionalExtensions;

namespace ReservationService.Domain.Users;

public record Details
{
    private readonly List<SocialNetwork> _socialNetworks = [];

    private Details(string description, string fio)
    {
        Description = description;
        Fio = fio;
    }

    public string Description { get; }

    public string Fio { get; }

    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks.AsReadOnly();

    public static Result<Details> Create(string description, string fio)
    {
        var details = new Details(description, fio);

        return Result.Success(details);
    }

    public Result AddSocialNetwork(SocialNetwork socialNetwork)
    {
        _socialNetworks.Add(socialNetwork);

        return Result.Success();
    }
}