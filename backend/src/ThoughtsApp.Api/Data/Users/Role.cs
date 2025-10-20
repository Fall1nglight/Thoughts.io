namespace ThoughtsApp.Api.Data.Users;

public class Role
{
    public const string Admin = "Admin";
    public const string Member = "Member";
    public const int AdminId = 1;
    public const int MemberId = 2;

    public required int Id { get; set; }
    public required string Name { get; set; }
}
