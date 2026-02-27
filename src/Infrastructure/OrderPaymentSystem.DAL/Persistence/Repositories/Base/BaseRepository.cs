using Mapster;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories.Base;
using OrderPaymentSystem.Shared.Specifications;
using System.Linq.Expressions;

namespace OrderPaymentSystem.DAL.Persistence.Repositories.Base;

/// <summary>
/// Generic репозиторий. Абстракция над DbContext
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Контекст для работы с БД
    /// </summary>
    protected readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// <see cref="DbSet{TEntity}"/> для работы с запросами для сущности
    /// </summary>
    protected readonly DbSet<TEntity> _table;

    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _table = _dbContext.Set<TEntity>();
    }

    /// <inheritdoc/>
    public async Task<TResult?> GetProjectedAsync<TResult>(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator<TEntity>
            .GetQuery(_table.AsNoTracking(), spec)
            .ProjectToType<TResult>()
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetFirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
    {
        return await SpecificationEvaluator<TEntity>
            .GetQuery(_table, spec)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<TEntity>> GetListBySpecAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
    {
        return await SpecificationEvaluator<TEntity>
            .GetQuery(_table, spec)
            .ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<TValue>> GetListValuesAsync<TValue>(
        ISpecification<TEntity> spec,
        Expression<Func<TEntity, TValue>> selector,
        CancellationToken ct = default)
    {
        return await SpecificationEvaluator<TEntity>
            .GetQuery(_table.AsNoTracking(), spec)
            .Select(selector)
            .ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<TValue?> GetValueAsync<TValue>(
        ISpecification<TEntity> spec,
        Expression<Func<TEntity, TValue>> selector,
        CancellationToken ct = default)
    {
        return await SpecificationEvaluator<TEntity>
            .GetQuery(_table.AsNoTracking(), spec)
            .Select(selector)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<TResult>> GetListProjectedAsync<TResult>(
        ISpecification<TEntity>? spec = null,
        CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator<TEntity>
            .GetQuery(_table.AsNoTracking(), spec)
            .ProjectToType<TResult>()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _dbContext.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        ValidateEntityOnNull(entity);

        await _table.AddAsync(entity, ct);

        return entity;
    }

    /// <inheritdoc/>
    public void Remove(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        _table.Remove(entity);
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

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
    {
        return await SpecificationEvaluator<TEntity>
            .GetQuery(_table, spec)
            .AnyAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<TEntity>> GetAll(CancellationToken ct = default)
    {
        return await _table.ToListAsync(ct);
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
