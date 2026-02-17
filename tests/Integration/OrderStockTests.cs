using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace OrderPaymentSystem.IntegrationTests
{
    public class OrderStockTests : BaseIntegrationTest
    {
        public OrderStockTests(IntegrationTestFactory factory) : base(factory) { }

        [Fact]
        public async Task CreateOrder_Should_Decrease_ProductStock_InDatabase()
        {
            // 1. Arrange: Создаем продукт в БД напрямую через DbContext, чтобы точно знать начальный сток
            const int initialStock = 100;
            const int orderQuantity = 5;
            int productId;

            using (var scope = Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var product = OrderPaymentSystem.Domain.Entities.Product.Create("Test Product", "Desc", 100, initialStock);
                db.Products.Add(product);
                await db.SaveChangesAsync();
                productId = product.Id;
            }

            await AuthenticateAsync("buyer", "User");

            // 2. Act: Создаем заказ через API
            var orderDto = new CreateOrderDto
            {
                DeliveryAddress = new Address("Country", "City", "Street", "12345"),
                OrderItems = new List<Application.DTOs.OrderItem.CreateOrderItemDto>
            {
                new() { ProductId = (int)productId, Quantity = orderQuantity }
            }
            };

            var response = await Client.PostAsJsonAsync("/api/v1/orders", orderDto);
            response.EnsureSuccessStatusCode();

            var order = await response.Content.ReadFromJsonAsync<OrderDto>();

            var dto = new CreatePaymentDto()
            {
                OrderId = order.Id,
                AmountPaid = 500,
                Method = Domain.Enum.PaymentMethod.GooglePay,
            };

            var response1 = await Client.PostAsJsonAsync("/api/v1/payments", dto);
            response1.EnsureSuccessStatusCode();
            var createdPayment = await response1.Content.ReadFromJsonAsync<PaymentDto>();

            await Client.PostAsJsonAsync($"/api/v1/orders/{order.Id}/process/{createdPayment.Id}", new object());

            // 3. Assert: Проверяем, что в базе сток уменьшился
            using (var scope = Factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var productFromDb = await db.Products.FindAsync(productId);

                productFromDb!.StockQuantity.Should().Be(initialStock - orderQuantity);
            }
        }
    }
}
