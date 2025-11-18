using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Repositories;

/// <summary>
/// Generic репозиторий. Абстракции над DbContext
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
/// <param name="dbContext"></param>
public class BaseRepository<TEntity>(ApplicationDbContext dbContext) : IBaseRepository<TEntity> where TEntity : class
{
    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable()
    {
        return dbContext.Set<TEntity>().AsQueryable();
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ValidateEntityOnNull(entity);

        await dbContext.AddAsync(entity, cancellationToken);

        return entity;
    }

    /// <inheritdoc/>
    public void Remove(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        dbContext.Remove(entity);
    }

    /// <inheritdoc/>
    public TEntity Update(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        dbContext.Update(entity);

        return entity;
    }

    /// <inheritdoc/>
    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        ValidateEntitiesOnNull(entities);

        dbContext.RemoveRange(entities);
    }

    /// <inheritdoc/>
    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        ValidateEntitiesOnNull(entities);

        dbContext.UpdateRange(entities);
    }

    /// <summary>
    /// Валидация сущности на NULL
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <exception cref="ArgumentNullException"></exception>
    private static void ValidateEntityOnNull(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity is null");
        }
    }

    /// <summary>
    /// Валидация коллекции сущностей на NULL
    /// </summary>
    /// <param name="entities"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private static void ValidateEntitiesOnNull(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentNullException(nameof(entities), "Entities is null");
        }
    }
}
