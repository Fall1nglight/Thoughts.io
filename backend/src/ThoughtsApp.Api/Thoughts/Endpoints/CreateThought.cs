using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class CreateThought : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("", Handle)
            .WithSummary("Creates a new thought")
            .WithRequestValidation<Request>();
    }

    public record Request(string Title, string Content, bool IsPublic);

    public record Response(Guid Id);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MinimumLength(5)
                .WithMessage("{PropertyName} must be at least {MinLength} characters long.")
                .MaximumLength(500)
                .WithMessage("{PropertyName} must not exceed {MaxLength} characters.");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MinimumLength(5)
                .WithMessage("{PropertyName} must be at least {MinLength} characters long.")
                .MaximumLength(500)
                .WithMessage("{PropertyName} must not exceed {MaxLength} characters.");

            RuleFor(x => x.IsPublic).NotNull().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Ok<Response>> Handle(
        Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var newThought = new Thought
        {
            UserId = claimsPrincipal.GetUserId(),
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            IsPublic = request.IsPublic,
        };

        await db.Thoughts.AddAsync(newThought, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(new Response(newThought.Id));
    }
}
