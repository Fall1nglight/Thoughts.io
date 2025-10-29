using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class DeleteThought : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapDelete("/{id}", Handle)
            .WithSummary("Deletes thought by id")
            .WithRequestValidation<Request>()
            .WithEnsureUserOwnsEntity<Thought, Request>(x => x.Id);
    }

    public record Request(Guid Id);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        }
    }

    private static async Task<Results<Ok, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext context,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var rowsDeleted = await context
            .Thoughts.Where(x => x.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsDeleted != 1)
            return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}
