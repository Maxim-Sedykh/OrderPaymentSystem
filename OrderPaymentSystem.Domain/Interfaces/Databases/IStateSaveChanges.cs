namespace OrderPaymentSystem.Domain.Interfaces.Databases;

/// <summary>
/// Интерфейс сохранения состояния сущностей
/// </summary>
public interface IStateSaveChanges
{
    /// <summary>
    /// Сохранить состояние сущности в БД
    /// </summary>
    /// <param name="cancellationToken">Токен для отмены операции</param>
    /// <returns>Количество обработанных кортежей в таблицах</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
