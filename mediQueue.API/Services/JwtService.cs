namespace mediQueue.API.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string JwtTokenGenerator()
    {
        // Implementation for JWT token generation
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = _configuration["Jwt:Key"];
        var expirationMinutes = Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"]);
        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

        return "hellow";

    }
}
