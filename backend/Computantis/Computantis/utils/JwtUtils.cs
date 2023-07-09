using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Computantis.database.models;
using Microsoft.IdentityModel.Tokens;
using R3TraceShared.utils;

namespace Computantis.utils;

public class JwtUtils
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secret;
    private readonly DateTime _notBefore;
    private readonly int _lifetime;
    
    public JwtUtils(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"]!;
        _audience = configuration["Jwt:Audience"]!;
        _secret = configuration["Jwt:Secret"]!;
        _lifetime = int.Parse(configuration["Jwt:Lifetime"]!);
        if (!String.IsNullOrEmpty(configuration["Jwt:NotBefore"]))
            _notBefore = DateTime.Parse(configuration["Jwt:NotBefore"]!);
    }

    public string GenerateJwtToken(User user, DateTime? expires = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            NotBefore = _notBefore,
            Subject = new ClaimsIdentity(new[] {new Claim("uid", user.Uid.ToString()!)}),
            Expires = expires ?? DateTime.UtcNow.AddSeconds(_lifetime),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Guid? ValidateJwtToken(string token)
    {
        if (String.IsNullOrEmpty(token))
            return null;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken) validatedToken;
            var userUid = Guid.Parse(jwtToken.Claims.First(x => x.Type == "uid").Value);

            return userUid;
        }
        catch
        {
            return null;
        }
    }

    public RefreshToken GenerateRefreshToken(Guid userUid, string ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            Token = GeneralUtils.RandomString(64),
            ExpiresAt = DateTime.UtcNow.AddMonths(6),
            CreatedAt = DateTime.UtcNow,
            CreatedIp = ipAddress,
            UserUid = userUid
        };

        return refreshToken;
    }
}