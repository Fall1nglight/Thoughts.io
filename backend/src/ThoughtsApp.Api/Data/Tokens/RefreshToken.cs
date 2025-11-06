using ThoughtsApp.Api.Data.Shared.Types;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Tokens;

public class RefreshToken : IEntity, IOwnedEntity
{
    // ids
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }

    // other properties
    public required string Token { get; set; }
    public DateTime ExpiresOnUtc { get; set; } = DateTime.Now.AddDays(1);

    // navigation properties
    public User User { get; set; } = null!;
}
