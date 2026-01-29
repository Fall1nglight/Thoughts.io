using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Authentication.Services;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Users;

public class UpdateUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPatch("/{id}", Handle)
            .WithSummary("Updates user")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid Id, [FromBody] Body Body);

    public record Body(string? Username, string? Email, string? Password);

    public record Response(string AccessToken);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(x => x.Body)
                .Must(x => x.Username != null || x.Email != null || x.Password != null)
                .WithMessage("Username, Email or Password is required.");

            When(
                x => x.Body.Email != null,
                () =>
                {
                    RuleFor(x => x.Body.Email)
                        .NotEmpty()
                        .WithMessage("Email address is required.")
                        .EmailAddress()
                        .WithMessage("Invalid email format.")
                        .MaximumLength(60)
                        .WithMessage("Email address must not exceed {MaxLength} characters.");
                }
            );

            When(
                x => x.Body.Username != null,
                () =>
                {
                    RuleFor(x => x.Body.Username)
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
                }
            );

            When(
                x => x.Body.Password != null,
                () =>
                {
                    RuleFor(x => x.Body.Password)
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
            );
        }
    }

    private static async Task<Results<Ok<Response>, BadRequest<ProblemDetails>>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        PasswordHasher passwordHasher,
        JwtProvider jwtProvider,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        if (request.Id != claimsPrincipal.GetUserId())
            return TypedResults.BadRequest(
                new ProblemDetails { Detail = "You are not authorized to update this user!" }
            );

        var user = await db.Users.Where(x => x.Id == request.Id).SingleAsync(cancellationToken);

        // ha megvan adva username, és nem egyezik meg az eddigivel => csak akkor foglalkozunk vele
        if (request.Body.Username != null && request.Body.Username != user.Username)
        {
            var isUsernameTaken = await db.Users.AnyAsync(
                x => x.Username == request.Body.Username,
                cancellationToken
            );

            if (isUsernameTaken)
                return TypedResults.BadRequest(
                    new ProblemDetails
                    {
                        Detail = "Username is already taken. Please try another one!",
                    }
                );

            user.Username = request.Body.Username;
        }

        // ha megvan adva email, és nem egyezik meg az eddigivel => csak akkor foglalkozunk vele
        if (request.Body.Email != null && request.Body.Email != user.Email)
        {
            var isEmailTaken = await db.Users.AnyAsync(
                x => x.Email == request.Body.Email,
                cancellationToken
            );

            if (isEmailTaken)
                return TypedResults.BadRequest(
                    new ProblemDetails
                    {
                        Detail = "Email is already taken. Please try another one!",
                    }
                );

            user.Email = request.Body.Email;
        }

        if (request.Body.Password != null)
        {
            var hashedPassword = passwordHasher.HashPassword(request.Body.Password);
            user.PasswordHash = hashedPassword;
        }

        await db.SaveChangesAsync(cancellationToken);

        // nem kell refreshTokent generálni, hiszen ez a route
        // csak bejelentkezett felhasználók számára érhető el => rendelkeznek refreshTokennel
        // elég, ha küldünk egy új accessTokent, amit a frontend feldolgoz majd
        // ha pedig lejárna az accessToken, a refreshToken egy új accessTokent fog generálni
        // amiben a már frissített adatok szerepelnek

        var accessToken = await jwtProvider.GenerateToken(user);
        var response = new Response(accessToken);
        return TypedResults.Ok(response);
    }
}
