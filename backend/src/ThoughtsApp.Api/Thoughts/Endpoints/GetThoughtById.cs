using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class GetThoughtById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/{id}", Handle)
            .WithSummary("Gets thought by id")
            .WithRequestValidation<Request>()
            .WithEnsureEntityIsAccessible<Api.Data.Thoughts.Thought, Request>(x => x.Id);
    }

    public record Request(Guid Id);

    public record User(Guid Id, string Username);

    public record Comment(int Count);

    public record Reaction(int Id, int Count);

    public record Thought(
        Guid Id,
        User User,
        int? UserReactionId,
        string Title,
        string Content,
        bool IsPublic,
        Comment Comments,
        List<Reaction> Reactions,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

    public record Response(Thought Thought);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Ok<Response>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        Guid? userId = null;

        if (claimsPrincipal.Identity?.IsAuthenticated == true)
            userId = claimsPrincipal.GetUserId();

        var thought = await db
            .Thoughts.Where(x => x.Id == request.Id)
            .AsNoTracking()
            .Select(t => new Thought(
                t.Id,
                new User(t.UserId, t.User.Username),
                userId == null
                    ? null
                    : t
                        .Reactions.Where(tr => tr.UserId == userId.Value)
                        .Select(tr => tr.Reaction.Id)
                        .SingleOrDefault(),
                t.Title,
                t.Content,
                t.IsPublic,
                new Comment(t.Comments.Count),
                t.Reactions.GroupBy(tr => tr.Reaction.Id)
                    .Select(group => new Reaction(group.Key, group.Count()))
                    .ToList(),
                t.CreatedAtUtc,
                t.UpdatedAtUtc
            ))
            .SingleAsync(cancellationToken);

        var response = new Response(thought);

        return TypedResults.Ok(response);
    }
}
