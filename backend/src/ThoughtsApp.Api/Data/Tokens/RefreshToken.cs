using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Tokens;

public class RefreshToken : IEntity, IOwnedEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
    public User User = null!;
}
