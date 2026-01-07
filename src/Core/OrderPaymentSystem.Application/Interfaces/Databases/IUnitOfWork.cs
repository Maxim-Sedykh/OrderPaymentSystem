using Microsoft.EntityFrameworkCore.Storage;

namespace OrderPaymentSystem.Application.Interfaces.Databases;

/// <summary>
/// Интерфейс для реализации паттерна Unit of work, для работы с транзакциями EF Core
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Сохранить состояние сущности в БД
    /// </summary>
    /// <param name="cancellationToken">Токен для отмены операции</param>
    /// <returns>Количество обработанных кортежей в таблицах</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать экземпляр транзакции.
    /// </summary>
    /// <param name="cancellationToken">Токен для отмены операции</param>
    /// <returns><see cref="IDbContextTransaction"/></returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
