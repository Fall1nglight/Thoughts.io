using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Authentication.Services;

internal sealed class JwtProviderOptions
{
    public required string Key { get; init; }
    public required int ExpirationInMinutes { get; init; }
}

internal sealed class JwtProvider
{
    private readonly AppDbContext _context;

    // fields
    private readonly IOptions<JwtProviderOptions> _options;

    // constructors
    public JwtProvider(IOptions<JwtProviderOptions> options, AppDbContext context)
    {
        _options = options;
        _context = context;
    }

    public async Task<string> GenerateToken(User user)
    {
        var secretKey = SecurityKey(_options.Value.Key);
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        // collect all roles assigned to the specific user
        // then map them to Claims
        var roles = await _context
            .UserRoles.Where(x => x.UserId == user.Id)
            .Select(x => new Claim(ClaimTypes.Role, x.Role.Name))
            .ToListAsync();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        ];

        claims.AddRange(roles);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),

            Expires = DateTime.UtcNow.AddMinutes(_options.Value.ExpirationInMinutes),
            SigningCredentials = credentials,
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public static SymmetricSecurityKey SecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
}
