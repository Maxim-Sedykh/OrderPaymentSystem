using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Interfaces.Services.Maintenance;
using OrderPaymentSystem.Application.Services.Auth;
using OrderPaymentSystem.Application.Services.BasketItems;
using OrderPaymentSystem.Application.Services.Maintenance;
using OrderPaymentSystem.Application.Services.Orders;
using OrderPaymentSystem.Application.Services.Payments;
using OrderPaymentSystem.Application.Services.Products;
using OrderPaymentSystem.Application.Services.Roles;

namespace OrderPaymentSystem.Application.DependencyInjection;

/// <summary>
/// Внедрение зависимостей слоя Application
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Зарегистрировать все зависимости из слоя Application
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    public static void AddApplication(this IServiceCollection services)
    {
        InitServices(services);
        InitMaintenanceServices(services);

        InitMapsterMapping(services);
    }

    /// <summary>
    /// Зарегистрировать сервисы с бизнес-логикой
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
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
        services.AddScoped<IUserRoleService, UserRoleService>();
    }

    /// <summary>
    /// Зарегистрировать сервисы которые работают по расписанию
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    private static void InitMaintenanceServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderMaintenanceService, OrderMaintenanceService>();
        services.AddScoped<ITokenMaintenanceService, TokenMaintenanceService>();
    }

    /// <summary>
    /// Зарегистрировать Mapster
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    private static void InitMapsterMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
