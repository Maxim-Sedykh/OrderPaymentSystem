using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Mapping;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.Application.Validations.FluentValidations.Order;
using OrderPaymentSystem.Application.Validations.FluentValidations.Payment;
using OrderPaymentSystem.Application.Validations.FluentValidations.Product;
using OrderPaymentSystem.Application.Validations.FluentValidations.Role;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Services;

namespace OrderPaymentSystem.Application.DependencyInjection
{
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
            services.AddScoped<IBasketService, BasketService>();
        }

        private static void InitAutoMapper(this IServiceCollection services)
        {
            var validatorsTypes = new List<Type>()
            {
                typeof(ProductMapping),
                typeof(RoleMapping),
                typeof(UserMapping),
                typeof(BasketMapping),
                typeof(PaymentMapping),
                typeof(OrderMapping)
            };

            foreach (var validatorType in validatorsTypes)
            {
                services.AddAutoMapper(validatorType);
            }
        }

        public static void InitFluentValidators(this IServiceCollection services)
        {
            var validatorsTypes = new List<Type>()
            {
                typeof(CreateProductValidator),
                typeof(UpdateProductValidator),
                typeof(CreatePaymentValidator),
                typeof(UpdatePaymentValidation),
                typeof(UpdateOrderValidation),
                typeof(CreateOrderValidation),
                typeof(LoginUserValidator),
                typeof(RegisterUserValidation),
                typeof(CreateRoleValidation),
                typeof(DeleteUserRoleValidation),
                typeof(UpdateUserRoleValidation),
                typeof(UserRoleValidation),
                typeof(RoleValidation)
            };

            foreach (var validatorType in validatorsTypes)
            {
                services.AddValidatorsFromAssembly(validatorType.Assembly);
            }
        }
    }
}
