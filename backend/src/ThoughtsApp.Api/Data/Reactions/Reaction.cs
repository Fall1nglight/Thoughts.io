namespace ThoughtsApp.Api.Data.Reactions;

public class Reaction
{
    public const string Like = "Like";
    public const string Dislike = "Dislike";
    public const string Laugh = "Laugh";
    public const int LikeId = 1;
    public const int DislikeId = 2;
    public const int LaughId = 3;

    // ids
    public required int Id { get; set; }

    // other properties
    public required string Name { get; set; }

    // navigation properties
    public List<ThoughtReaction> Reactions { get; set; } = [];
}
