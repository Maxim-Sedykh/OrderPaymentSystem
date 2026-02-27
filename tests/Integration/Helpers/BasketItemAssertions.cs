using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Basket;

namespace OrderPaymentSystem.IntegrationTests.Helpers;

/// <summary>
/// Лямбды для подтверждения утверждений в тестах для элементов корзины.
/// </summary>
public static class BasketItemAssertions
{
    /// <summary>
    /// Имеет ли коллекция элементов корзины определённый товар.
    /// </summary>
    /// <param name="basketItems">Элементы корзины.</param>
    /// <param name="productId">Идентификатор товара.</param>
    /// <param name="quantity">Количество товара.</param>
    public static void HaveProduct(this IEnumerable<BasketItemDto> basketItems, int productId, int quantity)
    {
        basketItems.Should().Contain(item => item.ProductId == productId && item.Quantity == quantity);
    }
}
