using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThoughtsApp.Api.Data.Reactions;

public class ThoughtReactionConfiguration : IEntityTypeConfiguration<ThoughtReaction>
{
    public void Configure(EntityTypeBuilder<ThoughtReaction> builder)
    {
        // ensuring users can only react to thoughts one time
        builder.HasKey(thoughtReaction => new
        {
            thoughtReaction.UserId,
            thoughtReaction.ThoughtId,
        });

        builder.Property(thoughtReaction => thoughtReaction.CreatedAtUtc).IsRequired();

        builder
            .HasOne(thoughtReaction => thoughtReaction.User)
            .WithMany()
            .HasForeignKey(thoughtReaction => thoughtReaction.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(thoughtReaction => thoughtReaction.Thought)
            .WithMany()
            .HasForeignKey(thoughtReaction => thoughtReaction.ThoughtId);

        builder
            .HasOne(thoughtReaction => thoughtReaction.Reaction)
            .WithMany()
            .HasForeignKey(thoughtReaction => thoughtReaction.ReactionId);
    }
}
