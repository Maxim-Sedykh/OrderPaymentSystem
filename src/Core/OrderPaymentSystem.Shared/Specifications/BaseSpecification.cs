using OrderPaymentSystem.Shared.Specifications.Helpers;
using System.Linq.Expressions;

namespace OrderPaymentSystem.Shared.Specifications;

public class BaseSpecification<T> : ISpecification<T>
{
    public BaseSpecification(Expression<Func<T, bool>> criteria) => Criteria = criteria;

    public BaseSpecification() => Criteria = x => true;

    public Expression<Func<T, bool>> Criteria { get; private set; }

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

    public BaseSpecification<T> And(Expression<Func<T, bool>> newCriteria)
    {
        if (newCriteria == null) return this;

        if (Criteria == null)
        {
            Criteria = newCriteria;
            return this;
        }

        var parameter = Criteria.Parameters.Single();

        var visitor = new ParameterRebinder(parameter);
        var newBody = visitor.Visit(newCriteria.Body);

        var combinedBody = Expression.AndAlso(Criteria.Body, newBody);

        Criteria = Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
        return this;
    }
}