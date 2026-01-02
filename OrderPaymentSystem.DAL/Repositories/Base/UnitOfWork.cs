using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Domain.Interfaces.Databases;

namespace OrderPaymentSystem.DAL.Repositories.Base;

/// <summary>
/// Unit of work. Сервис для работы с транзакциями EF Core
/// </summary>
/// <param name="context"></param>
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    /// <inheritdoc/>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
