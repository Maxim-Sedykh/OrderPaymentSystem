using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Producer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Producer.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddProducer(this IServiceCollection services)
        {
            services.AddScoped<IMessageProducer, Producer>();
        }
    }
}
