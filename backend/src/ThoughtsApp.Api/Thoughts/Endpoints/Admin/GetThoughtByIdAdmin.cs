using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Thoughts.Endpoints.Admin;

public class GetThoughtByIdAdmin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/{id}", Handle)
            .WithSummary("Gets thought by id")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid Id);

    public record User(Guid Id, string Username);

    public record Comment(int Count);

    public record Reaction(int Id, int Count);

    public record Thought(
        Guid Id,
        User User,
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

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var thought = await db
            .Thoughts.Where(x => x.Id == request.Id)
            .AsNoTracking()
            .Select(t => new Thought(
                t.Id,
                new User(t.UserId, t.User.Username),
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
            .SingleOrDefaultAsync(cancellationToken);

        if (thought == null)
            return TypedResults.NotFound();

        var response = new Response(thought);

        return TypedResults.Ok(response);
    }
}
