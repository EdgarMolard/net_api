using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MonPremierProjet.Services;

namespace MonPremierProjet.Tests;

public class TokenBuilderTests
{
    [Fact]
    public async Task BuildTokenAsync_ReturnsTokenAcceptedByConfiguredValidation()
    {
        const string key = "EdgarSecurityServiceDevelopmentKey32!";
        const string issuer = "EdgarSecurityService";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = key,
                ["Jwt:Issuer"] = issuer
            })
            .Build();
        var builder = new TokenBuilder(configuration);

        var result = await builder.BuildTokenAsync();
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        handler.ValidateToken(
            result.AccessToken,
            validationParameters,
            out var validatedToken);

        var jwt = Assert.IsType<JwtSecurityToken>(validatedToken);
        Assert.Equal("development-user", jwt.Subject);
        Assert.True(result.Expiration > DateTime.UtcNow);
    }
}
