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
            .WithEnsureEntityIsAccessible<Thought, Request>(x => x.Id);
    }

    public record Request(Guid Id);

    public record Response(
        Guid Id,
        string Username,
        string Title,
        string Content,
        bool IsPublic,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

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
            .Include(x => x.User)
            .Select(x => new Response(
                x.Id,
                x.User.Username,
                x.Title,
                x.Content,
                x.IsPublic,
                x.CreatedAtUtc,
                x.UpdatedAtUtc
            ))
            .SingleAsync(cancellationToken);

        return TypedResults.Ok(thought);
    }
}
