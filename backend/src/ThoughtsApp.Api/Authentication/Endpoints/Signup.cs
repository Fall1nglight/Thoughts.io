using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication.Services;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Tokens;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Authentication.Endpoints;

public class Signup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/signup", Handle)
            .WithSummary("Registers a new user")
            .WithRequestValidation<Request>();
    }

    public record Request(string Email, string Username, string Password);

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

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(5)
                .WithMessage("Username must be at least {MinLength} characters long.")
                .MaximumLength(30)
                .WithMessage("Username must not exceed {MaxLength} characters.")
                .Matches("^[a-zA-Z0-9_]*$")
                .WithMessage(
                    "Username can only contain letters, numbers, and underscores (no spaces)."
                );

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

    private static async Task<Results<Ok<Response>, BadRequest<ProblemDetails>>> Handle(
        Request request,
        AppDbContext context,
        PasswordHasher passwordHasher,
        JwtProvider jwtProvider,
        RefreshTokenProvider refreshTokenProvider,
        CancellationToken cancellationToken
    )
    {
        var isEmailTaken = await context.Users.AnyAsync(
            x => x.Email == request.Email,
            cancellationToken
        );

        if (isEmailTaken)
            return TypedResults.BadRequest(
                new ProblemDetails
                {
                    Detail = "Email address is already taken. Please try another one!",
                }
            );

        var isUsernameTaken = await context.Users.AnyAsync(
            x => x.Username == request.Username,
            cancellationToken
        );

        if (isUsernameTaken)
            return TypedResults.BadRequest(
                new ProblemDetails { Detail = "Username is already taken. Please try another one!" }
            );

        var hashedPassword = passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = hashedPassword,
        };

        await context.Users.AddAsync(user, cancellationToken);
        var userRole = new UserRole { UserId = user.Id, RoleId = Role.MemberId };
        await context.UserRoles.AddAsync(userRole, cancellationToken);

        var accessToken = await jwtProvider.GenerateToken(user);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.Now.AddDays(1),
        };

        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(new Response(accessToken, refreshToken.Token));
    }
}
