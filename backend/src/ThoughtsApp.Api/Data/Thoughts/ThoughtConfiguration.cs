using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThoughtsApp.Api.Data.Thoughts;

public class ThoughtConfiguration : IEntityTypeConfiguration<Thought>
{
    public void Configure(EntityTypeBuilder<Thought> builder)
    {
        builder.HasKey(thought => thought.Id);

        builder.Property(thought => thought.UserId).IsRequired();
        builder.Property(thought => thought.Title).HasMaxLength(50).IsRequired();
        builder.Property(thought => thought.Content).HasMaxLength(250).IsRequired();
        builder.Property(thought => thought.IsPublic).HasDefaultValue(true).IsRequired();
        builder.Property(thought => thought.CreatedAtUtc).IsRequired();
        builder.Property(thought => thought.UpdatedAtUtc).IsRequired();
    }
}
