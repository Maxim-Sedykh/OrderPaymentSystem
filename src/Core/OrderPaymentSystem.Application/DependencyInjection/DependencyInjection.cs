using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Mapping;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.Application.Validations.FluentValidations.Order;
using OrderPaymentSystem.Application.Validations.FluentValidations.Payment;
using OrderPaymentSystem.Application.Validations.FluentValidations.Product;
using OrderPaymentSystem.Application.Validations.FluentValidations.Role;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;

namespace OrderPaymentSystem.Application.DependencyInjection;

/// <summary>
/// Внедрение зависимостей слоя Application
/// </summary>
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        InitAutoMapper(services);

        InitServices(services);

        InitFluentValidators(services);
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

    private static void InitAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile<ProductMapping>();
            config.AddProfile<RoleMapping>();
            config.AddProfile<UserMapping>();
            config.AddProfile<BasketMapping>();
            config.AddProfile<PaymentMapping>();
            config.AddProfile<OrderMapping>();
        });
    }

    public static void InitFluentValidators(this IServiceCollection services)
    {
        var validatorsTypes = new List<Type>()
        {
            typeof(CreateProductValidator),
            typeof(UpdateProductValidator),
            typeof(CreatePaymentValidator),
            typeof(CompletePaymentValidator),
            typeof(UpdateOrderValidation),
            typeof(CreateOrderValidator),
            typeof(LoginUserValidator),
            typeof(RegisterUserValidator),
            typeof(CreateRoleValidator),
            typeof(DeleteUserRoleValidation),
            typeof(UpdateUserRoleValidator),
            typeof(UpdateRoleValidator)
        };

        foreach (var validatorType in validatorsTypes)
        {
            services.AddValidatorsFromAssembly(validatorType.Assembly);
        }
    }
}
