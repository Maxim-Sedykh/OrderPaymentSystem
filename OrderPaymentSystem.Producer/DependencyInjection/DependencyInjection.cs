using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Producer.Interfaces;

namespace OrderPaymentSystem.Producer.DependencyInjection
{
    /// <summary>
    /// Внедрение зависимостей Producer
    /// </summary>
    public static class DependencyInjection
    {
        public static void AddProducer(this IServiceCollection services)
        {
            services.AddScoped<IMessageProducer, Producer>();
        }
    }
}
