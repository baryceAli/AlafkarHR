
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.DDD;
using System.Security.Claims;

namespace Shared.Data.Interceptors;

public class AuditableEntityInterceptor: SaveChangesInterceptor
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value??throw new UnauthorizedAccessException("User is not authenticated");
        if (context == null) return;
        foreach(var entry in context.ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = "Bashir";
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {

                entry.Entity.ModifiedBy = user;
                entry.Entity.ModifiedAt = DateTime.UtcNow;
            }
        }
    }
}
public static class Extentions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r=> 
            r.TargetEntry !=null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State==EntityState.Added || r.TargetEntry.State==EntityState.Modified));
}