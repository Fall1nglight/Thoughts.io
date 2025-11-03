using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Thoughts;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Comments;

public class Comment : IEntity, IOwnedEntity
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public required Guid ThoughtId { get; set; }
    public Thought Thought { get; set; } = null!;

    public required string Content { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.MinValue;
}
