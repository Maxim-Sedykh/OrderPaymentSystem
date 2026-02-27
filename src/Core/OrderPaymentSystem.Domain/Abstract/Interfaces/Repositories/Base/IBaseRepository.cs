using OrderPaymentSystem.Shared.Specifications;
using System.Linq.Expressions;

namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;

/// <summary>
/// Интерфейс для generic репозитория. Абстракции над DbContext
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public interface IBaseRepository<TEntity>
{
    /// <summary>
    /// Получить все записи из таблицы.
    /// Вызывайте осторожно.
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Коллекция сущностей</returns>
    Task<List<TEntity>> GetAll(CancellationToken ct = default);

    /// <summary>
    /// Есть ли сущность с условием из спецификации
    /// </summary>
    /// <param name="spec">Спецификация</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>True - есть</returns>
    Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken ct = default);

    /// <summary>
    /// Получить первую сущность с условием из спецификации
    /// Если такой нет - то null
    /// </summary>
    /// <param name="spec">Спецификация</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns></returns>
    Task<TEntity?> GetFirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken ct = default);

    /// <summary>
    /// Получить коллекцию сущностей по условию из спецификации
    /// </summary>
    /// <param name="spec">Спецификация</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Коллекция сущностей</returns>
    Task<List<TEntity>> GetListBySpecAsync(ISpecification<TEntity> spec, CancellationToken ct = default);

    /// <summary>
    /// Получить проецированную сущность по условию из спецификации
    /// </summary>
    /// <typeparam name="TResult">Тип, в который проецируем сущность</typeparam>
    /// <param name="spec">Спецификация</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Проецированная сущность</returns>
    Task<TResult?> GetProjectedAsync<TResult>(
        ISpecification<TEntity> spec,
        CancellationToken ct = default);

    /// <summary>
    /// Получить коллекцию проецированных сущностей по условию из спецификации
    /// </summary>
    /// <typeparam name="TResult">Тип, в который проецируем сущность</typeparam>
    /// <param name="spec">Спецификация</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Коллекция проецированных сущностей</returns>
    Task<List<TResult>> GetListProjectedAsync<TResult>(
        ISpecification<TEntity>? spec = null,
        CancellationToken ct = default);

    /// <summary>
    /// Получить коллекцию полей из сущностей по условию из спецификации
    /// </summary>
    /// <typeparam name="TValue">Тип, в который проецируем сущность</typeparam>
    /// <param name="spec">Спецификация</param>
    /// <param name="selector">Делегат для выбора полей которые селектим</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Коллекция значений сущностей</returns>
    Task<List<TValue>> GetListValuesAsync<TValue>(
        ISpecification<TEntity> spec,
        Expression<Func<TEntity, TValue>> selector,
        CancellationToken ct = default);


    /// <summary>
    /// Получить коллекцию значений из сущностей по условию из спецификации
    /// </summary>
    /// <typeparam name="TValue">Тип, в который проецируем сущность</typeparam>
    /// <param name="spec">Спецификация</param>
    /// <param name="selector">Делегат для выбора полей которые селектим</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Значение из сущности</returns>
    Task<TValue?> GetValueAsync<TValue>(
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
