using ThoughtsApp.Api.Data.Shared.Types;

namespace ThoughtsApp.Api.Data.Users;

public class UserRole : IOwnedEntity
{
    // ids
    public required int RoleId { get; set; }
    public required Guid UserId { get; set; }

    // naviation properties
    public Role Role { get; set; } = null!;
    public User User { get; set; } = null!;
}
