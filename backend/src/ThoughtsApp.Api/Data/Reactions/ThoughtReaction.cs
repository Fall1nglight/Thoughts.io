using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Thoughts;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Reactions;

public class ThoughtReaction : IOwnedEntity
{
    public required Guid ThoughtId { get; set; }
    public required int ReactionId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.Now;
    public User User { get; set; } = null!;
    public Thought Thought { get; set; } = null!;
    public Reaction Reaction { get; set; } = null!;
    public required Guid UserId { get; set; }
}
