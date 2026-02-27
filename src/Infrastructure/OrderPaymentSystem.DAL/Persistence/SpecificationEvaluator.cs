using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.DAL.Persistence;

/// <summary>
/// Переводчик спецификаций в <see cref="IQueryable{T}"/>
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public static class SpecificationEvaluator<T> where T : class
{
    /// <summary>
    /// Получить запрос из спецификации
    /// </summary>
    /// <param name="inputQuery">Начальный запрос</param>
    /// <param name="spec">Спецификация</param>
    /// <returns>Обновлённый запрос</returns>
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T>? spec)
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
