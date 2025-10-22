using System.Security.Cryptography;

namespace ThoughtsApp.Api.Authentication.Services;

public class RefreshTokenProvider
{
    public string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}
