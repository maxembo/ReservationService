namespace ReservationService.Domain.Users;

public record SocialNetwork
{
    public SocialNetwork()
    {
    
    }
    
    public string Name { get; set; }
    
    public string Link { get; set; }
}