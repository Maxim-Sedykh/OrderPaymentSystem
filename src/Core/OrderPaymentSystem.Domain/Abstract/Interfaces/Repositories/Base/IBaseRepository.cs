using OrderPaymentSystem.Shared.Specifications;
using System.Linq.Expressions;

namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;

/// <summary>
/// Интерфейс для generic репозитория. Абстракции над DbContext
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public interface IBaseRepository<TEntity>
{
    Task<List<TEntity>> GetAll(CancellationToken ct = default);

    Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken ct = default);

    Task<TEntity> GetFirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken ct = default);

    Task<List<TEntity>> GetListBySpecAsync(ISpecification<TEntity> spec, CancellationToken ct = default);

    Task<TResult> GetProjectedAsync<TResult>(
        ISpecification<TEntity> spec,
        CancellationToken ct = default);

    Task<List<TResult>> GetListProjectedAsync<TResult>(
        ISpecification<TEntity> spec = null,
        CancellationToken ct = default);

    Task<List<TValue>> GetListValuesAsync<TValue>(
        ISpecification<TEntity> spec,
        Expression<Func<TEntity, TValue>> selector,
        CancellationToken ct = default);

    Task<TValue> GetValueAsync<TValue>(
        ISpecification<TEntity> spec,
        Expression<Func<TEntity, TValue>> selector,
        CancellationToken ct = default);

    /// <summary>
    /// Создать сущность в БД
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="ct">Токен для отмены операции</param>
    /// <returns>Созданную сущность</returns>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default);

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
