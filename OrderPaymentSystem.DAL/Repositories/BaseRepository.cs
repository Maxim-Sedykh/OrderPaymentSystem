using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Repositories;

public class BaseRepository<TEntity>(ApplicationDbContext dbContext) : IBaseRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> GetAll()
    {
        return dbContext.Set<TEntity>().AsQueryable();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync();
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        await dbContext.AddAsync(entity);

        return entity;
    }

    public void Remove(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        dbContext.Remove(entity);
    }

    public TEntity Update(TEntity entity)
    {
        ValidateEntityOnNull(entity);

        dbContext.Update(entity);

        return entity;
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        ValidateEntitiesOnNull(entities);

        dbContext.RemoveRange(entities);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        ValidateEntitiesOnNull(entities);

        dbContext.UpdateRange(entities);
    }

    private void ValidateEntityOnNull(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity is null");
        }
    }

    private void ValidateEntitiesOnNull(IEnumerable<TEntity> entities)
    {
        if (entities is null)
        {
            throw new ArgumentNullException(nameof(entities), "Entities is null");
        }
    }
}
