using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using System.Reflection;
using OrderPaymentSystem.DAL.Extensions;

namespace OrderPaymentSystem.DAL.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        ProcessAuditableEntities(dbContext);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ProcessAuditableEntities(DbContext dbContext)
    {
        DateTime currentTimeUtc = DateTime.UtcNow;

        foreach (var entry in dbContext.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State != EntityState.Added && entry.State != EntityState.Modified && !entry.HasChangedOwnedType())
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                SetPropertyDateTime(entry.Entity, nameof(IAuditable.CreatedAt), currentTimeUtc);
            }

            if (entry.State == EntityState.Modified || entry.State == EntityState.Added || entry.HasChangedOwnedType())
            {
                SetPropertyDateTime(entry.Entity, nameof(IAuditable.UpdatedAt), currentTimeUtc);
            }
        }
    }

    private static void SetPropertyDateTime(IAuditable entity, string propertyName, DateTime value)
    {
        PropertyInfo property = entity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) 
            ?? entity.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (property != null && property.CanWrite)
        {
            property.SetValue(entity, value);
        }
    }
}
