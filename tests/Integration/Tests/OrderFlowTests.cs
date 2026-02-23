using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.IntegrationTests.Base;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Tests;

public class OrderFlowTests : BaseIntegrationTest
{
    public OrderFlowTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Complete_Order_Flow_Should_Work_Correctly()
    {
        //Arrange & Act
        await AuthenticateAsync(TestConstants.AdminLogin, TestConstants.AdminRole);

        var productDto = new CreateProductDto("Laptop", "Pro", 2000m, 10);
        var createdProduct = await Factory.CreateProductAsync(productDto, Client);
        createdProduct.Should().NotBeNull();
        int productId = createdProduct.Id;

        var basketDto = new CreateBasketItemDto(productId, 2);
        var basketAddResponse = await Client.PostAsJsonAsync(TestConstants.ApiBasketV1, basketDto);
        basketAddResponse.EnsureSuccessStatusCode();

        var orderDto = new CreateOrderDto
        {
            DeliveryAddress = new Address("Russia", "Moscow", "Red Square", "101000"),
            OrderItems =
            [
                new() { ProductId = productId, Quantity = 2 }
            ]
        };

        var orderResponse = await Client.PostAsJsonAsync(TestConstants.ApiOrdersV1, orderDto);
        orderResponse.EnsureSuccessStatusCode();

        var createdOrderId = await orderResponse.Content.ReadFromJsonAsync<long>();

        var getResponse = await Client.GetAsync($"{TestConstants.ApiOrdersV1}/{createdOrderId}");

        getResponse.EnsureSuccessStatusCode();

        var getCreatedOrder = await getResponse.Content.ReadFromJsonAsync<OrderDto>();

        getCreatedOrder.Should().NotBeNull();

        // Assert
        getCreatedOrder.TotalAmount.Should().Be(4000m); // 2000 * 2
        getCreatedOrder.Items.Should().HaveCount(1).And.Contain(item => item.ProductId == productId && item.Quantity == 2);
        getCreatedOrder.Status.Should().Be(OrderStatus.Pending);
    }
}
