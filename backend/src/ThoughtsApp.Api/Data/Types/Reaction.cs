namespace ThoughtsApp.Api.Data.Types;

public class Reaction
{
    public const string Like = "Like";
    public const string Dislike = "Dislike";
    public const string Laugh = "Laugh";
    public const int LikeId = 1;
    public const int DislikeId = 2;
    public const int LaughId = 3;

    public required int Id { get; set; }
    public required string Name { get; set; }
}
