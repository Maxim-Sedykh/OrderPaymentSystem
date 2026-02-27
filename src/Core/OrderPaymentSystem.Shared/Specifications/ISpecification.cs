using System.Linq.Expressions;

namespace OrderPaymentSystem.Shared.Specifications;

/// <summary>
/// Интерфейс для спецификации
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Условие фильтрации
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Include действия для подгрузки данных
    /// </summary>
    List<Func<IQueryable<T>, IQueryable<T>>> IncludeActions { get; }

    /// <summary>
    /// Делать ли текущий запрос не отслеживающимся ChangeTracker'ом
    /// </summary>
    bool IsAsNoTracking { get; }
}
