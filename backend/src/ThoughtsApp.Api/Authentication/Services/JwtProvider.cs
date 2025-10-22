using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Authentication.Services;

internal sealed class JwtProviderOptions
{
    public required string Key { get; init; }
    public required int ExpirationInMinutes { get; init; }
}

internal sealed class JwtProvider
{
    // fields
    private readonly IOptions<JwtProviderOptions> _options;

    // constructors
    public JwtProvider(IOptions<JwtProviderOptions> options)
    {
        _options = options;
    }

    // [x] jwt options
    // [x] string generateToken(User user)
    // [x] string generateSecurityKey(string key)
    // [ ] service lifetime?

    public string GenerateToken(User user)
    {
        var secretKey = SecurityKey(_options.Value.Key);

        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    // role?
                ]
            ),

            Expires = DateTime.UtcNow.AddMinutes(_options.Value.ExpirationInMinutes),
            SigningCredentials = credentials,
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public static SymmetricSecurityKey SecurityKey(string key) =>
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
}
