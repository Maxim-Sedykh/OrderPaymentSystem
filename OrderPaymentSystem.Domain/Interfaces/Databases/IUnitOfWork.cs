using Microsoft.EntityFrameworkCore.Storage;

namespace OrderPaymentSystem.Domain.Interfaces.Databases;

/// <summary>
/// Интерфейс для реализации паттерна Unit of work, для работы с транзакциями EF Core
/// </summary>
public interface IUnitOfWork : IStateSaveChanges
{
    /// <summary>
    /// Создать экземпляр транзакции.
    /// </summary>
    /// <param name="cancellationToken">Токен для отмены операции</param>
    /// <returns><see cref="IDbContextTransaction"/></returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
