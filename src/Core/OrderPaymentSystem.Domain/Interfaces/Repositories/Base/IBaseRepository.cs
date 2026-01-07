using OrderPaymentSystem.Domain.Interfaces.Entities;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories.Base;

/// <summary>
/// Интерфейс для generic репозитория. Абстракции над DbContext
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public interface IBaseRepository<TEntity> : IStateSaveChanges
{
    /// <summary>
    /// Получить все сущности в виде <see cref="IQueryable"/>
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/></returns>
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// Создать сущность в БД
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="cancellationToken">Токен для отмены операции</param>
    /// <returns>Созданную сущность</returns>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Пометить сущность как Modified
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns>Сущность</returns>
    TEntity Update(TEntity entity);

    /// <summary>
    /// Пометить сущность как Deleted
    /// </summary>
    /// <param name="entity">Сущность</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Пометить коллекцию сущностей как Deleted
    /// </summary>
    /// <param name="entities">Коллекция сущностей</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Пометить коллекцию сущностей как Modified
    /// </summary>
    /// <param name="entities">Сущность</param>
    /// <returns>Сущность</returns>
    void UpdateRange(IEnumerable<TEntity> entities);
}
