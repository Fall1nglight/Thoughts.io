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
    // fields
    private readonly IOptions<JwtProviderOptions> _options;
    private readonly AppDbContext _context;

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

        // collect all assigned roles to the specific user
        List<string> roleNames = await _context
            .UserRoles.Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .ToListAsync();

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        ];

        // map each roleName to Claim, then add it to claims
        claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),

            Expires = DateTime.UtcNow.AddMinutes(_options.Value.ExpirationInMinutes),
            SigningCredentials = credentials,
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public static SymmetricSecurityKey SecurityKey(string key) =>
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
}
