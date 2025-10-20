using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThoughtsApp.Api.Data.Users;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);
        builder.HasIndex(role => role.Name).IsUnique();

        builder.Property(role => role.Name).HasMaxLength(50).IsRequired();

        builder.HasData(
            [
                new Role { Id = Role.AdminId, Name = Role.Admin },
                new Role { Id = Role.MemberId, Name = Role.Member },
            ]
        );
    }
}
