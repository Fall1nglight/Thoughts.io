using ThoughtsApp.Api.Data.Shared.Types;

namespace ThoughtsApp.Api.Data.Users;

public class UserRole : IOwnedEntity
{
    public required int RoleId { get; set; }
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public required Guid UserId { get; set; }
}
