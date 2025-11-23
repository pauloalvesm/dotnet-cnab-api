using CNAB.Application.Interfaces.Account;
using CNAB.Application.Services.Account;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace CNAB.Application.Test.Account;

public class TokenServiceTest
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ITokenService _tokenService;

    public TokenServiceTest()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["Jwt:key"]).Returns("pnmeumo#ultramicroscopicospj@silico#vulcano$cocioticorsserver");
        _configurationMock.Setup(c => c["TokenConfiguration:Issuer"]).Returns("Paulo_Issuer");
        _configurationMock.Setup(c => c["TokenConfiguration:Audience"]).Returns("Paulo_Audience");
        _configurationMock.Setup(c => c["TokenConfiguration:ExpireHours"]).Returns("1");

        _tokenService = new TokenService(_configurationMock.Object);
    }

    [Fact(DisplayName = "GenerateToken - Should return valid token for valid email")]
    public void TokenService_GenerateToken_ShouldReturnValidTokenForValidEmail()
    {
        // Arrange
        var email = "user@localhost";

        // Act
        var result = _tokenService.GenerateToken(email);

        // Assert
        result.Should().NotBeNull();
        result.Authenticated.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.Expiration.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(5));
        result.Message.Should().Be("Token JWT OK");
    }

    [Fact(DisplayName = "GenerateToken - Should add delete permission claim for sdmin email")]
    public void TokenService_GenerateToken_ShouldAddDeletePermissionClaimForAdminEmail()
    {
        // Arrange
        var adminEmail = "admin@localhost";

        // Act
        var result = _tokenService.GenerateToken(adminEmail);

        // Assert
        result.Should().NotBeNull();
        result.Authenticated.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.Expiration.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(5));
        result.Message.Should().Be("Token JWT OK");

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(result.Token);
        jwtToken.Claims.Should().Contain(c => c.Type == "DeletePermission" && c.Value == "true");
    }

    [Fact(DisplayName = "GenerateToken - Should throw exception for invalid configuration")]
    public void TokenService_GenerateToken_ShouldThrowExceptionForInvalidConfiguration()
    {
        // Arrange
        var email = "user@localhost";
        _configurationMock.Setup(c => c["Jwt:key"]).Returns((string)null);

        // Act
        Action act = () => _tokenService.GenerateToken(email);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}