using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication.Services;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Authentication.Endpoints;

public class RenewToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/refresh-token", Handle)
            .WithSummary("Provides new access token")
            .WithRequestValidation<Request>();
    }

    public record Request(string Token);

    public record Response(string AccessToken, string RefreshToken);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Token).Length(44).WithMessage("Invalid token.");
        }
    }

    private static async Task<Results<Ok<Response>, BadRequest>> Handle(
        Request request,
        AppDbContext context,
        JwtProvider jwtProvider,
        RefreshTokenProvider refreshTokenProvider,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = await context
            .RefreshTokens.Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Token == request.Token, cancellationToken);

        if (refreshToken == null)
            return TypedResults.BadRequest();

        // if the user tries to use an expired token
        // invalidate every refresh token associated with the user
        // to prevent xss attacks
        if (refreshToken.ExpiresOnUtc < DateTime.Now)
        {
            await context
                .RefreshTokens.Where(x => x.UserId == refreshToken.UserId)
                .ExecuteDeleteAsync(cancellationToken);

            return TypedResults.BadRequest();
        }

        var accessToken = await jwtProvider.GenerateToken(refreshToken.User);
        refreshToken.Token = refreshTokenProvider.GenerateRefreshToken();
        refreshToken.ExpiresOnUtc = DateTime.Now.AddDays(1);

        await context.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(new Response(accessToken, refreshToken.Token));
    }
}
