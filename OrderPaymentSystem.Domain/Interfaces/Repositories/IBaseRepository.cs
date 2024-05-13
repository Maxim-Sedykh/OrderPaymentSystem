using OrderPaymentSystem.Domain.Interfaces.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> : IStateSaveChanges
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> CreateAsync(TEntity entity);

        TEntity Update(TEntity entity);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        void UpdateRange(IEnumerable<TEntity> entities);

        Task<TEntity> GetOne<T>(T id);
    }
}
