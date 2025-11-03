using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThoughtsApp.Api.Data.Comments;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).IsRequired().HasMaxLength(150);
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        // todo | delete user's comments before deleting user

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Thought)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ThoughtId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
