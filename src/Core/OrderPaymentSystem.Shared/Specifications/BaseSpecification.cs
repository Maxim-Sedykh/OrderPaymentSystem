using OrderPaymentSystem.Shared.Specifications.Helpers;
using System.Linq.Expressions;

namespace OrderPaymentSystem.Shared.Specifications;

/// <summary>
/// Базовый класс спецификации
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseSpecification<T> : ISpecification<T>
{
    /// <summary>
    /// Создание спецификации с фильтрацией
    /// </summary>
    /// <param name="criteria"></param>
    public BaseSpecification(Expression<Func<T, bool>> criteria) => Criteria = criteria;

    /// <summary>
    /// Создание спецификации без фильтрации
    /// </summary>
    public BaseSpecification() => Criteria = x => true;

    /// <summary>
    /// Фильтрация
    /// </summary>
    public Expression<Func<T, bool>> Criteria { get; private set; }

    /// <summary>
    /// Набор Include
    /// </summary>
    public List<Func<IQueryable<T>, IQueryable<T>>> IncludeActions { get; } = new();

    /// <summary>
    /// Делать ли текущий запрос не отслеживающимся ChangeTracker'ом
    /// </summary>
    public bool IsAsNoTracking { get; private set; } = false;

    /// <summary>
    /// Добавить Include в запрос спецификации
    /// </summary>
    /// <param name="includeAction"></param>
    /// <returns></returns>
    public BaseSpecification<T> AddInclude(Func<IQueryable<T>, IQueryable<T>> includeAction)
    {
        IncludeActions.Add(includeAction);

        return this;
    }

    /// <summary>
    /// Убрать отслеживание сущностей, полученных с запроса спецификации
    /// </summary>
    /// <returns></returns>
    public BaseSpecification<T> ApplyAsNoTracking()
    {
        IsAsNoTracking = true;

        return this;
    }

    /// <summary>
    /// Обновить фильтрации внутри одной спецификации
    /// </summary>
    /// <param name="newCriteria">Новая фильтрация</param>
    /// <returns>Обновлённая спецификация</returns>
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