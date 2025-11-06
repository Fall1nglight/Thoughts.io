using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Thoughts;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Reactions;

public class ThoughtReaction : IOwnedEntity
{
    // ids
    public required int ReactionId { get; set; }
    public required Guid ThoughtId { get; set; }
    public required Guid UserId { get; set; }

    // other properties
    public DateTime CreatedAtUtc { get; set; } = DateTime.Now;

    // navigation properties
    public Reaction Reaction { get; set; } = null!;
    public Thought Thought { get; set; } = null!;
    public User User { get; set; } = null!;
}
