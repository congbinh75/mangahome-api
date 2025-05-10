using MangaHome.Core.Models;
using MangaHome.Core.Abstractions;
using MangaHome.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MangaHome.Infrastructure.Contexts;

public class MangaHomeDbContext: DbContext
{
    public DbSet<User> Users { get; set; }

    public bool IsSoftDeleteIgnored { get; set; } = false;

    private readonly IDateTimeService _dateTimeService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly ILogger<MangaHomeDbContext> _logger;

    public MangaHomeDbContext(
        DbContextOptions<MangaHomeDbContext> options, 
        IDateTimeService dateTimeService,
        IRequestInfoService requestInfoService,
        ILogger<MangaHomeDbContext> logger) : base(options)
    {
        _dateTimeService = dateTimeService;
        _requestInfoService = requestInfoService;
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mangahome");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityTypeConfiguration).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = _dateTimeService.GetUtcNow();
        var currentUserId = _requestInfoService.GetCurrentUserId();

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is BaseEntity entity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = now;
                        entity.UpdatedAt = now;
                        entity.CreatedBy = currentUserId;
                        entity.UpdatedBy = currentUserId;
                        break;
                    case EntityState.Modified:
                        Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = currentUserId;
                        break;
                    case EntityState.Deleted: //For soft-delete
                        if (IsSoftDeleteIgnored) break;
                        Entry(entity).State = EntityState.Modified;
                        Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = currentUserId;
                        entity.IsDeleted = true;
                        entity.DeletedAt = now;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}