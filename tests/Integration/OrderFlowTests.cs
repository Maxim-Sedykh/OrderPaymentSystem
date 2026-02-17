using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace OrderPaymentSystem.IntegrationTests
{
    public class OrderFlowTests : BaseIntegrationTest
    {
        public OrderFlowTests(IntegrationTestFactory factory) : base(factory) { }

        [Fact]
        public async Task Complete_Order_Flow_Should_Work_Correctly()
        {
            // 1. Авторизуемся
            await AuthenticateAsync("admin", "Admin");

            // 2. Создаем продукт (как админ, чтобы он был в базе)
            // Для этого теста временно сменим токен на админский или используем существующий
            var productDto = new CreateProductDto("Laptop", "Pro", 2000m, 10);
            await Client.PostAsJsonAsync("/api/v1/products", productDto);

            // Получаем ID продукта (предположим, он 1)
            var products = await Client.GetFromJsonAsync<List<ProductDto>>("/api/v1/products");
            var productId = products.First().Id;

            // 3. Добавляем в корзину
            var basketDto = new CreateBasketItemDto((int)productId, 2);
            var basketResponse = await Client.PostAsJsonAsync("/api/v1/basket", basketDto);
            basketResponse.EnsureSuccessStatusCode();

            // 4. Оформляем заказ
            var orderDto = new CreateOrderDto
            {
                DeliveryAddress = new Address("Russia", "Moscow", "Red Square", "101000"),
                OrderItems = new List<Application.DTOs.OrderItem.CreateOrderItemDto>
            {
                new() { ProductId = (int)productId, Quantity = 2 }
            }
            };

            var orderResponse = await Client.PostAsJsonAsync("/api/v1/orders", orderDto);

            // Assert
            orderResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var createdOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
            createdOrder!.TotalAmount.Should().Be(4000m); // 2000 * 2
        }
    }
}
