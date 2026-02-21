using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Basket;

namespace OrderPaymentSystem.IntegrationTests.Helpers;

public static class BasketItemAssertions
{
    public static void HaveProduct(this IEnumerable<BasketItemDto> basketItems, int productId, int quantity)
    {
        basketItems.Should().Contain(item => item.ProductId == productId && item.Quantity == quantity);
    }
}
