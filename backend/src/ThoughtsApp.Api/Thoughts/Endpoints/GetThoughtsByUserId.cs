using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class GetThoughtsByUserId : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/user/{userId}", Handle)
            .WithSummary("Gets thoughts by userId")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid UserId);

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

    public record Response(List<Thought> Thoughts);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("{PropertyName} is required.");
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

        // todo
        // 1. ha nincs bejelentkezve csak a publikus gondolatokat adjuk vissza
        // 2. ha be van jelentkezve a felhasználó és a saját gondolatait kéri -> mindet visszaadjuk
        // 3. ha be van jelentkezve és nem a saját gondolatait kéri -> csak a publikusokat adjuk
        // 1. és 3. ugyan az => össze kell vonni

        var userThoughts = await db
            .Thoughts.Where(t =>
                t.UserId == request.UserId
                && (
                    userId == null || userId.Value != request.UserId
                        ? t.IsPublic == true
                        : t.IsPublic == true || t.IsPublic == false
                )
            )
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
            .ToListAsync(cancellationToken);

        var response = new Response(userThoughts);

        return TypedResults.Ok(response);
    }
}
