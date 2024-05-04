using OrderPaymentSystem.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            ValidateEntityOnNull(entity);

            return entity;
        }

        public void Remove(TEntity entity)
        {
            ValidateEntityOnNull(entity);

            _dbContext.Remove(entity);
        }

        public TEntity Update(TEntity entity)
        {
            ValidateEntityOnNull(entity);

            _dbContext.Update(entity);

            return entity;
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            ValidateEntitiesOnNull(entities);

            _dbContext.RemoveRange(entities);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            ValidateEntitiesOnNull(entities);

            _dbContext.UpdateRange(entities);
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
}
