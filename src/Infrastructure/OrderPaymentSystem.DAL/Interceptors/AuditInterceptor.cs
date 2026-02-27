using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;

namespace OrderPaymentSystem.DAL.Interceptors;

/// <summary>
/// Перехватчик запросов. Интерсептор EF Core.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// При вызове <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> перехватываем запрос.
    /// Чтобы у <see cref="EntityState.Modified"/> сущностей проставить <see cref="IAuditable.UpdatedAt"/>
    /// а у созданных <see cref="EntityState.Added"/> проставить <see cref="IAuditable.CreatedAt"/>
    /// Вместе с CreatedAt проставляется и UpdatedAt
    /// </summary>
    /// <param name="eventData">Информация о текущем запросе</param>
    /// <param name="result">Результат интерсептора</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entries = dbContext.ChangeTracker.Entries<IAuditable>()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
            .ToList();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
