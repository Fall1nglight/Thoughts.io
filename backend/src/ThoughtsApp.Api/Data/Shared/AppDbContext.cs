using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Data.Reactions;
using ThoughtsApp.Api.Data.Thoughts;
using ThoughtsApp.Api.Data.Tokens;
using ThoughtsApp.Api.Data.Users;

namespace ThoughtsApp.Api.Data.Shared;

public class AppDbContext : DbContext
{
    private const string DefaultSchema = "thoughtsApp";

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Thought> Thoughts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<ThoughtReaction> ThoughtReactions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // apply configuration from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.HasDefaultSchema(DefaultSchema);

        // configures default behaviour (ie.: identity tables)
        // could be omitted but it is a best practice
        base.OnModelCreating(modelBuilder);
    }
}
