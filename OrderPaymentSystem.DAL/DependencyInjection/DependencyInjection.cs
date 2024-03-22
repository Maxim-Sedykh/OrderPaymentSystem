using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.DAL.Interceptors;
using OrderPaymentSystem.DAL.Repositories;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Builder;

namespace OrderPaymentSystem.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Внедрение зависимостей слоя DAL
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgresSQL");

            services.AddSingleton<AuditInterceptor>();
            services.AddDbContext<ApplicationDbContext>(options => 
            { 
                options.UseNpgsql(connectionString); 
            });

            services.InitRepositories();
            services.InitUnitOfWork();
        }

        private static void InitRepositories(this IServiceCollection services)
        {
            var types = new List<Type>()
            {
                typeof(User),
                typeof(Order),
                typeof(Payment),
                typeof(Product),
                typeof(UserToken),
                typeof(UserRole),
                typeof(Role),
                typeof(Basket)
            };

            foreach (var type in types)
            {
                var interfaceType = typeof(IBaseRepository<>).MakeGenericType(type);
                var implementationType = typeof(BaseRepository<>).MakeGenericType(type);
                services.AddScoped(interfaceType, implementationType);
            }
        }

        private static void InitUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
