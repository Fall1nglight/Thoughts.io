using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Tokens;

public class RefreshToken : IEntity, IOwnedEntity
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresOnUtc { get; set; } = DateTime.Now.AddDays(1);
    public User User { get; set; } = null!;
}
