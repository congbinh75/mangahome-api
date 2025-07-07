using MangaHome.Core.Models;
using MangaHome.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace MangaHome.Infrastructure.Contexts;

public class MangaHomeDbContext: DbContext
{
    public DbSet<User> Users { get; set; }

    public bool IsSoftDeleteIgnored { get; set; } = false;

    public MangaHomeDbContext(DbContextOptions<MangaHomeDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mangahome");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityTypeConfiguration).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is BaseEntity entity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = now;
                        entity.UpdatedAt = now;
                        break;
                    case EntityState.Modified:
                        Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        entity.UpdatedAt = now;
                        break;
                    case EntityState.Deleted: //For soft-delete
                        if (IsSoftDeleteIgnored) break;
                        Entry(entity).State = EntityState.Modified;
                        Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        entity.UpdatedAt = now;
                        entity.IsDeleted = true;
                        entity.DeletedAt = now;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}