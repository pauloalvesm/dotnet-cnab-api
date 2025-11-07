using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CNAB.Application.DTOs.Account;
using CNAB.Application.Interfaces.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CNAB.Application.Services.Account;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public UserTokenDto GenerateToken(string email)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, email),
            new Claim("myPC", "keyboard"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (email == "admin@localhost")
        {
            claims.Add(new Claim("DeletePermission", "true"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenExpiration = _configuration["TokenConfiguration:ExpireHours"];
        var expiration = DateTime.UtcNow.AddHours(double.Parse(tokenExpiration));

        var token = new JwtSecurityToken(
            issuer: _configuration["TokenConfiguration:Issuer"],
            audience: _configuration["TokenConfiguration:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new UserTokenDto()
        {
            Authenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            Message = "Token JWT OK"
        };
    }
}