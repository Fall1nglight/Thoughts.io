using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Comments.Endpoints;

public class GetComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("{thoughtId}/comments/{id}", Handle)
            .WithSummary("Gets a single comment for the specified thought.")
            .WithEnsureEntityIsAccessible<Thought, Request>(x => x.ThoughtId);
    }

    public record Request(Guid ThoughtId, Guid Id);

    public record User(Guid Id, string Username);

    public record Comment(
        Guid Id,
        string Content,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc,
        User User
    );

    public record Response(Comment Comment);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var comment = await db
            .Comments.Where(x => x.ThoughtId == request.ThoughtId && x.Id == request.Id)
            .Select(x => new Comment(
                x.Id,
                x.Content,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                new User(x.UserId, x.User.Username)
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (comment == null)
            return TypedResults.NotFound();

        var response = new Response(comment);
        return TypedResults.Ok(response);
    }
}
