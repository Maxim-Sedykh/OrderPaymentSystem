using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Specifications;

namespace OrderPaymentSystem.Application.DependencyInjection;

/// <summary>
/// Внедрение зависимостей слоя Application
/// </summary>
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        InitServices(services);

        services.AddScoped<ISpecification<Order>, BaseSpecification<Order>();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserTokenService, UserTokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IBasketItemService, BasketItemService>();
    }
}
