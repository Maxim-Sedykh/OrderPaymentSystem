using System.Linq.Expressions;

namespace OrderPaymentSystem.Shared.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification(Expression<Func<T, bool>> criteria) => Criteria = criteria;

        public Expression<Func<T, bool>> Criteria { get; }

        public List<Func<IQueryable<T>, IQueryable<T>>> IncludeActions { get; } = new();

        public bool IsAsNoTracking { get; private set; } = false;

        public BaseSpecification<T> AddInclude(Func<IQueryable<T>, IQueryable<T>> includeAction)
        {
            IncludeActions.Add(includeAction);

            return this;
        }

        public BaseSpecification<T> ApplyAsNoTracking()
        {
            IsAsNoTracking = true;

            return this;
        }
    }
}
