using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Data.Users;

public class User : IEntity
{
    public Guid Id { get; init; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.MinValue;
    public List<Thought> Thoughts { get; set; } = [];
}
