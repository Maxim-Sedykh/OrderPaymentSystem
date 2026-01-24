using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.DAL.Persistence;

public static class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        if (spec == null)
        {
            return query;
        }

        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        query = spec.IncludeActions.Aggregate(query, (current, action) => action(current));

        if (spec.IsAsNoTracking)
            query = query.AsNoTracking();

        return query;
    }
}
