using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using mediQueue.API.Model.Entity;
using System.IdentityModel.Tokens.Jwt;

namespace mediQueue.API.Services;

public class JwtService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public string JwtTokenGenerator(User user)
    {
        // FIX 1: Change "Jwt" to "JwtConfig" to match your json
        var issuer = _configuration["JwtConfig:Issuer"];
        var audience = _configuration["JwtConfig:Audience"];
        var key = _configuration["JwtConfig:Key"];

        // FIX 2: Match the exact spelling from your JSON ("TokenValidtyMins")
        // Or better yet, fix the spelling in JSON to "TokenValidityMins"
        var expirationMinutes = Convert.ToDouble(_configuration["JwtConfig:TokenValidtyMins"]);

        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.Email),
            // You might want to add ID here too
             new Claim("id", user.Id.ToString()),
             new Claim(ClaimTypes.Role, user.Role)
        }),

            Issuer = issuer,
            Audience = audience,
            Expires = expiration,
            // Now "key" will not be null, so this won't crash
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
