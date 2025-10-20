using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThoughtsApp.Api.Data.Reactions;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.HasKey(reaction => reaction.Id);
        builder.HasIndex(reaction => reaction.Name).IsUnique();

        builder.Property(reaction => reaction.Name).HasMaxLength(15).IsRequired();

        builder.HasData(
            [
                new Reaction { Id = Reaction.LikeId, Name = Reaction.Like },
                new Reaction { Id = Reaction.DislikeId, Name = Reaction.Dislike },
                new Reaction { Id = Reaction.LaughId, Name = Reaction.Laugh },
            ]
        );
    }
}
