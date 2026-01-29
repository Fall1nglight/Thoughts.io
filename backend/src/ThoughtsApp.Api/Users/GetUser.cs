using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Users;

public class GetUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/{id}", Handle)
            .WithSummary("Gets user details by id")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid Id);

    public record User(Guid Id, string Username, DateTime CreatedAtUtc, Stats Stats);

    public record Stats(Thought Thoughts, Comment Comments, List<Reaction> Reactions);

    public record Comment(int Count);

    public record Thought(int Count);

    public record Reaction(int Id, int Count);

    public record Response(User User);

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
        CancellationToken cancellationToken
    )
    {
        var user = await db
            .Users.Where(x => x.Id == request.Id)
            .AsNoTracking()
            .Select(x => new User(
                x.Id,
                x.Username,
                x.CreatedAtUtc,
                new Stats(
                    new Thought(x.Thoughts.Count),
                    new Comment(x.Comments.Count),
                    x.Reactions.GroupBy(r => r.ReactionId)
                        .Select(g => new Reaction(g.Key, g.Count()))
                        .ToList()
                )
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (user == null)
            return TypedResults.NotFound();

        var respone = new Response(user);
        return TypedResults.Ok(respone);
    }
}
