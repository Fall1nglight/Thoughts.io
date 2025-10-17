using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Data.Types;

namespace ThoughtsApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    private const string DefaultSchema = "thoughtsApp";
    public DbSet<User> Users { get; set; }
    public DbSet<Thought> Thoughts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<ThoughtReaction> ThoughtReactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsersTable(modelBuilder);
        ConfigureThoughtsTable(modelBuilder);
        ConfigureRolesTable(modelBuilder);
        ConfigureUserRolesTable(modelBuilder);
        ConfigureReactionsTable(modelBuilder);
        ConfigureThoughtReactionsTable(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultSchema);

        // configures default behaviour (ie.: identity tables)
        // could be omitted but it is a best practice
        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUsersTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<User>();

        builder.HasIndex(user => user.Id).IsUnique();
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

    private static void ConfigureThoughtsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Thought>();

        builder.HasIndex(thought => thought.Id).IsUnique();
        builder.Property(thought => thought.UserId).IsRequired();
        builder.Property(thought => thought.Title).HasMaxLength(50).IsRequired();
        builder.Property(thought => thought.Content).HasMaxLength(250).IsRequired();
        builder.Property(thought => thought.IsPublic).HasDefaultValue(true).IsRequired();
        builder.Property(thought => thought.CreatedAtUtc).IsRequired();
        builder.Property(thought => thought.UpdatedAtUtc).IsRequired();
    }

    private static void ConfigureRolesTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Role>();

        builder.HasIndex(role => role.Id).IsUnique();
        builder.HasIndex(role => role.Name).IsUnique();

        builder.Property(role => role.Name).HasMaxLength(50).IsRequired();

        builder.HasData(
            [
                new Role { Id = Role.AdminId, Name = Role.Admin },
                new Role { Id = Role.MemberId, Name = Role.Member },
            ]
        );
    }

    private static void ConfigureUserRolesTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<UserRole>();

        builder.HasKey(userRole => new { userRole.UserId, userRole.RoleId });

        builder
            .HasOne(userRole => userRole.User)
            .WithMany()
            .HasForeignKey(userRole => userRole.UserId);

        builder
            .HasOne(userRole => userRole.Role)
            .WithMany()
            .HasForeignKey(userRole => userRole.RoleId);
    }

    private static void ConfigureReactionsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Reaction>();

        builder.HasIndex(reaction => reaction.Id).IsUnique();
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

    private static void ConfigureThoughtReactionsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<ThoughtReaction>();

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
