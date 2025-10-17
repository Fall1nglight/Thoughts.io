using Microsoft.EntityFrameworkCore;

namespace ThoughtsApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}
