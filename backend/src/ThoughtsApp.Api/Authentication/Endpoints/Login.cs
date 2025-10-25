using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication.Services;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Tokens;

namespace ThoughtsApp.Api.Authentication.Endpoints;

public class Login : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/login", Handle)
            .WithSummary("Authenticates users")
            .WithRequestValidation<Request>();
    }

    public record Request(string Email, string Password);

    public record Response(string AccessToken, string RefreshToken);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MaximumLength(60)
                .WithMessage("Email address must not exceed {MaxLength} characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least {MinLength} characters long.")
                .MaximumLength(60)
                .WithMessage("Password must not exceed {MaxLength} characters.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
        Request request,
        AppDbContext context,
        PasswordHasher passwordHasher,
        JwtProvider jwtProvider,
        RefreshTokenProvider refreshTokenProvider,
        CancellationToken cancellationToken
    )
    {
        // todo | solve "timing attack"
        // => if the provided email is INVALID the API responds super fast
        // => if the provided email is VALID the API verifies the password
        //  which takes time, so the user can guess if an email address
        //  exists or not from the response time

        var user = await context.Users.SingleOrDefaultAsync(
            x => x.Email == request.Email,
            cancellationToken
        );

        if (user == null)
            return TypedResults.Unauthorized();

        var isPasswordValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

        if (!isPasswordValid)
            return TypedResults.Unauthorized();

        var accessToken = await jwtProvider.GenerateToken(user);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenProvider.GenerateRefreshToken(),
        };

        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(new Response(accessToken, refreshToken.Token));
    }
}
