using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Interfaces.Databases.Repositories.Base;

namespace OrderPaymentSystem.DAL.Repositories.Base;

/// <summary>
/// Generic репозиторий. Абстракция над DbContext
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<TEntity> _table;

    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _table = _dbContext.Set<TEntity>();
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable()
    {
        return _table.AsQueryable();
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ValidateEntityOnNull(entity);

        await _table.AddAsync(entity, cancellationToken);

        return entity;
    }

    /// <inheritdoc/>
    public void Remove(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        _table.Remove(entity);
    }

    /// <inheritdoc/>
    public TEntity Update(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        _table.Update(entity);

        return entity;
    }

    /// <inheritdoc/>
    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        ValidateEntitiesOnNull(entities);

        _table.RemoveRange(entities);
    }

    /// <inheritdoc/>
    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        ValidateEntitiesOnNull(entities);

        _table.UpdateRange(entities);
    }

    /// <summary>
    /// Валидация сущности на NULL
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected static void ValidateEntityOnNull(TEntity entity)
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
    protected static void ValidateEntitiesOnNull(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentNullException(nameof(entities), "Entities is null");
        }
    }
}
