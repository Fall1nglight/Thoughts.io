namespace ThoughtsApp.Api.Data.Types;

public class Thought : IEntity, IOwnedEntity
{
    public Guid Id { get; init; }
    public required Guid UserId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.Now;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.MinValue;
    public User User { get; set; } = null!;
}
