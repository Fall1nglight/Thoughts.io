using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class UpdateThought : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPut("/{id}", Handle)
            .WithSummary("Updates a thought")
            .WithRequestValidation<Request>()
            .WithEnsureUserOwnsEntity<Thought, Request>(x => x.Id);
    }

    public record Body(string Title, string Content, bool IsPublic);

    public record Request(Guid Id, [FromBody] Body Body);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Body.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MinimumLength(5)
                .WithMessage("Title must be at least {MinLength} characters long.")
                .MaximumLength(500)
                .WithMessage("Title must not exceed {MaxLength} characters.");

            RuleFor(x => x.Body.Content)
                .NotEmpty()
                .WithMessage("Content is required.")
                .MinimumLength(5)
                .WithMessage("Content must be at least {MinLength} characters long.")
                .MaximumLength(500)
                .WithMessage("Content must not exceed {MaxLength} characters.");

            RuleFor(x => x.Body.IsPublic).NotNull().WithMessage("IsPublic is required.");
        }
    }

    private static async Task<Ok> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        // SingleAsync over SingleOrDefaultAsync
        // => because the "WithEnsureEntityOwned" filter ensures that the entity exists
        // and owned by the user
        var thought = await db.Thoughts.SingleAsync(x => x.Id == request.Id, cancellationToken);

        thought.Title = request.Body.Title.Trim();
        thought.Content = request.Body.Content.Trim();
        thought.IsPublic = request.Body.IsPublic;
        thought.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
