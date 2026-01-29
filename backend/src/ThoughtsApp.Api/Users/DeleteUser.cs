using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Users;

public class DeleteUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapDelete("/{id}", Handle)
            .WithSummary("Deletes user")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid Id);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Results<NoContent, BadRequest<ProblemDetails>>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        if (request.Id != claimsPrincipal.GetUserId())
            return TypedResults.BadRequest(
                new ProblemDetails { Detail = "You are not authorized to update this user!" }
            );

        await db
            .ThoughtReactions.Where(x => x.UserId == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await db
            .RefreshTokens.Where(x => x.UserId == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await db.Comments.Where(x => x.UserId == request.Id).ExecuteDeleteAsync(cancellationToken);

        await db.Users.Where(x => x.Id == request.Id).ExecuteDeleteAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
