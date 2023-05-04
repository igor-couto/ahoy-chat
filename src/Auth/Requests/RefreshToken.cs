namespace AhoyAuth.Requests;

public class RefreshToken
{
    public string Token { get; init; }
    public DateTime Created { get; init; } =  DateTime.UtcNow;
    public DateTime Expires { get; init; }
}