using TagaAral.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace TagaAral.Infrastructure.Data;

public class TagaAralDbContext : DbContext
{
    public TagaAralDbContext(DbContextOptions<TagaAralDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
