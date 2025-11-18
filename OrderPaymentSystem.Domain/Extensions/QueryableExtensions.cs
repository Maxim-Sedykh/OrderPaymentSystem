using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace OrderPaymentSystem.Domain.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TDestination> AsProjected<TSource, TDestination>(this IQueryable<TSource> source, IMapper mapper)
        {
            return source.ProjectTo<TDestination>(mapper.ConfigurationProvider);
        }
    }
}
