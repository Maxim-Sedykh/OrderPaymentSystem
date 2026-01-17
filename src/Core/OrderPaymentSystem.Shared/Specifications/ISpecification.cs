using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OrderPaymentSystem.Shared.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }

        List<Func<IQueryable<T>, IQueryable<T>>> IncludeActions { get; }

        bool IsAsNoTracking { get; }
    }
}
