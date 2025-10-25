using System.Security.Claims;

namespace ThoughtsApp.Api.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var value = claimsPrincipal
            .Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (value == null || !Guid.TryParse(value, out var guid))
            throw new Exception("UserId is invalid.");

        return guid;
    }
}
