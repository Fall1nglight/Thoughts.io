namespace ThoughtsApp.Api.Data.Types;

public class ThoughtReaction : IOwnedEntity
{
    public required Guid UserId { get; set; }
    public required Guid ThoughtId { get; set; }
    public required int ReactionId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.Now;
    public User User { get; set; } = null!;
    public Thought Thought { get; set; } = null!;
    public Reaction Reaction { get; set; } = null!;
}
