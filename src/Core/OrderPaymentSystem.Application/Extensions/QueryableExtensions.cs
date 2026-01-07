using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace OrderPaymentSystem.Application.Extensions;

/// <summary>
/// Расширения для IQueryable<T>
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Обёртка над ProjectTo из AutoMapper
    /// Позволяет делать проекцию объектов на уровне SQL-запросов, не выгружая все колонки в память
    /// </summary>
    /// <typeparam name="TSource">Изначальный объект</typeparam>
    /// <typeparam name="TDestination">Объект в который маппим</typeparam>
    /// <param name="source">Изначальный объект</param>
    /// <param name="mapper">Экземпляр маппера <see cref="IMapper"/></param>
    /// <returns></returns>
    public static IQueryable<TDestination> AsProjected<TSource, TDestination>(this IQueryable<TSource> source, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return source.ProjectTo<TDestination>(mapper.ConfigurationProvider);
    }
}
