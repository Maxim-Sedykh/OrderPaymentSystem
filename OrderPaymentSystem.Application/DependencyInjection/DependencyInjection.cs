﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Mapping;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Application.Validations;
using OrderPaymentSystem.Application.Validations.FluentValidations;
using OrderPaymentSystem.Domain.Dto.Product;
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
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductMapping));

            InitServices(services);
        }

        private static void InitServices(this IServiceCollection services)
        {
            services.AddScoped<IProductValidator, ProductValidator>();
            services.AddScoped<IValidator<CreateProductDto>, CreateProductValidator>();
            services.AddScoped<IValidator<ProductDto>, UpdateProductValidator>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserTokenService, UserTokenService>();
        }
    }
}
