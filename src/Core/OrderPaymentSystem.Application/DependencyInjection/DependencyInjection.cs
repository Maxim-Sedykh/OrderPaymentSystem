using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services;

namespace OrderPaymentSystem.Application.DependencyInjection;

/// <summary>
/// Внедрение зависимостей слоя Application
/// </summary>
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        InitServices(services);

        InitMapsterMapping(services);
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserTokenService, UserTokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderItemService, OrderItemService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IBasketItemService, BasketItemService>();
    }

    private static void InitMapsterMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
