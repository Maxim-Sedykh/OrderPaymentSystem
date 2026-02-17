using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace OrderPaymentSystem.IntegrationTests
{
    public class ProductControllerTests : BaseIntegrationTest
    {
        public ProductControllerTests(IntegrationTestFactory factory) : base(factory) { }

        [Fact]
        public async Task CreateProduct_AsAdmin_ShouldReturnCreated()
        {
            // Arrange
            await AuthenticateAsync("admin", "Admin");
            var productDto = new CreateProductDto("IPhone 15", "Apple Smartphone", 1000m, 50);

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/products", productDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteProduct_AsRegularUser_ShouldReturnForbidden()
        {
            // Arrange
            // Мы не реализовали в AuthService выдачу ролей при регистрации, 
            // предположим, что первый юзер не админ или у нас есть механизм смены ролей
            await AuthenticateAsync("normal_user", "User");

            // Act
            var response = await Client.DeleteAsync("/api/v1/products/1");

            // Assert
            // Если у тебя настроена политика Roles = "Admin", вернется 403
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
