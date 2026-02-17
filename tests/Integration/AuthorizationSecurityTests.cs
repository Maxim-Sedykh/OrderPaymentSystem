using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace OrderPaymentSystem.IntegrationTests
{
    public class SecurityTests : BaseIntegrationTest
    {
        public SecurityTests(IntegrationTestFactory factory) : base(factory) { }

        [Fact]
        public async Task AdminAction_ByRegularUser_ShouldReturnForbidden()
        {
            // 1. Arrange: Логинимся как обычный пользователь
            await AuthenticateAsync("common_user", "User");

            // 2. Act: Пытаемся создать продукт (это действие только для Admin)
            var createDto = new CreateProductDto("Hacker Tool", "Should fail", 999m, 1);
            var response = await Client.PostAsJsonAsync("/api/v1/products", createDto);

            // 3. Assert: Ожидаем 403 Forbidden
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
