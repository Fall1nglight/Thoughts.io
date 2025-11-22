using ThoughtsApp.Api.Data.Comments;
using ThoughtsApp.Api.Data.Reactions;
using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Thoughts;

public class Thought : IEntity, IOwnedEntity, IAccessibleEntity
{
    // ids
    public Guid Id { get; init; }
    public required Guid UserId { get; set; }

    // other properties
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required bool IsPublic { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.MinValue;

    // navigation properties
    public User User { get; set; } = null!;
    public List<ThoughtReaction> Reactions { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
}
