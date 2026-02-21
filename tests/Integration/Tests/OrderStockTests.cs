using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.IntegrationTests.Base;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Tests;

public class OrderStockTests : BaseIntegrationTest
{
    public OrderStockTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateOrder_Should_Decrease_ProductStock_InDatabase()
    {
        // Arrange
        const int initialStock = 100;
        const int orderQuantity = 5;
        int productId;

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var product = Domain.Entities.Product.Create("Test Stock Product", "Desc", 100, initialStock);
            db.Products.Add(product);
            await db.SaveChangesAsync();
            productId = product.Id;
        }

        await AuthenticateAsync("buyer", TestConstants.UserRole, TestConstants.CommonUserPassword);

        // Act
        var orderDto = new CreateOrderDto
        {
            DeliveryAddress = new Address("Country", "City", "Street", "12345"),
            OrderItems =
            [
                new() { ProductId = productId, Quantity = orderQuantity }
            ]
        };

        var orderResponse = await Client.PostAsJsonAsync(TestConstants.ApiOrdersV1, orderDto);
        orderResponse.EnsureSuccessStatusCode();
        var createdOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
        createdOrder.Should().NotBeNull();

        // Act
        var paymentDto = new CreatePaymentDto()
        {
            OrderId = createdOrder.Id,
            AmountPaid = 500,
            Method = Domain.Enum.PaymentMethod.GooglePay,
        };

        var paymentResponse = await Client.PostAsJsonAsync(TestConstants.ApiPaymentsV1, paymentDto);
        paymentResponse.EnsureSuccessStatusCode();
        var createdPayment = await paymentResponse.Content.ReadFromJsonAsync<PaymentDto>();
        createdPayment.Should().NotBeNull();

        var processResponse = await Client.PostAsJsonAsync($"{TestConstants.ApiOrdersV1}/{createdOrder.Id}/payments/{createdPayment.Id}/complete", new object());
        processResponse.EnsureSuccessStatusCode();

        // Assert
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var productFromDb = await db.Products.FindAsync(productId);
            productFromDb.Should().NotBeNull();
            productFromDb!.StockQuantity.Should().Be(initialStock - orderQuantity);
        }
    }
}
