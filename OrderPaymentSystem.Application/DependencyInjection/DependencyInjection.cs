using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Mapping;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Application.Validations.EntityValidators;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.Application.Validations.FluentValidations.Order;
using OrderPaymentSystem.Application.Validations.FluentValidations.Payment;
using OrderPaymentSystem.Application.Validations.FluentValidations.Product;
using OrderPaymentSystem.Application.Validations.FluentValidations.Role;
using OrderPaymentSystem.Application.Validations.FluentValidations.UserRole;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.DependencyInjection
{
    /// <summary>
    /// Внедрение зависимостей слоя Application
    /// </summary>
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductMapping));
            services.AddAutoMapper(typeof(RoleMapping));
            services.AddAutoMapper(typeof(UserMapping));

            InitServices(services);

            InitFluentValidators(services);
            InitEntityValidators(services);
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

        private static void InitEntityValidators(this IServiceCollection services)
        {
            services.AddScoped<IUserValidator, UserValidator>();
            services.AddScoped<IRoleValidator, RoleValidator>();
            services.AddScoped<IBaseValidator<Order>, OrderValidator>();
            services.AddScoped<IBaseValidator<Basket>, BasketValidator>();
            services.AddScoped<IPaymentValidator, PaymentValidator>();
            services.AddScoped<IProductValidator, ProductValidator>();
        }
    }
}
