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
                .WithMessage("Title is required.")
                .MinimumLength(5)
                .WithMessage("Title must be at least {MinLength} characters long.")
                .MaximumLength(500)
                .WithMessage("Title must not exceed {MaxLength} characters.");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content is required.")
                .MinimumLength(5)
                .WithMessage("Content must be at least {MinLength} characters long.")
                .MaximumLength(500)
                .WithMessage("Content must not exceed {MaxLength} characters.");

            RuleFor(x => x.IsPublic).NotNull().WithMessage("IsPublic is required.");
        }
    }

    private static async Task<Ok<Response>> Handle(
        Request request,
        AppDbContext context,
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

        await context.Thoughts.AddAsync(newThought, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(new Response(newThought.Id));
    }
}
