using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThoughtsApp.Api.Data.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.Username).HasMaxLength(30).IsRequired();
        builder.Property(user => user.Email).HasMaxLength(60).IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(60).IsRequired();
        builder.Property(user => user.CreatedAtUtc).IsRequired();
        builder.Property(user => user.UpdatedAtUtc).IsRequired();

        builder
            .HasMany(user => user.Thoughts)
            .WithOne(thought => thought.User)
            .HasForeignKey(thought => thought.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
