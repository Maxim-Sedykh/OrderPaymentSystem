using Microsoft.Extensions.DependencyInjection;

namespace OrderPaymentSystem.Consumer.DependencyInjection
{
    /// <summary>
    /// Внедрение зависимостей Consumer
    /// </summary>
    public static class DependencyInjection
    {
        public static void AddConsumer(this IServiceCollection services)
        {
            services.AddHostedService<RabbitMqListener>();
        }
    }
}
