using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.DAL.Interceptors;
using OrderPaymentSystem.DAL.Repositories;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MSSQL");

            services.AddSingleton<DateInterceptor>();
            services.AddDbContext<ApplicationDbContext>(options => 
            { 
                options.UseSqlServer(connectionString); 
            });
            services.InitRepositories();
        }

        private static void InitRepositories(this IServiceCollection services)
        {
            var types = new List<Type>()
            {
                typeof(User),
                typeof(Customer),
                typeof(Employee),
                typeof(Order),
                typeof(Payment),
                typeof(Product),
                typeof(Report),
                typeof(Warehouse)
            };

            foreach (var type in types)
            {
                var interfaceType = typeof(IBaseRepository<>).MakeGenericType(type);
                var implementationType = typeof(BaseRepository<>).MakeGenericType(type);
                services.AddScoped(interfaceType, implementationType);
            }
        }
    }
}
