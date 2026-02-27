namespace OrderPaymentSystem.Application.DTOs.Basket;

/// <summary>
/// Модель данных для создания элемента корзины
/// </summary>
/// <param name="ProductId">Id товара</param>
/// <param name="Quantity">Количество товара</param>
public record CreateBasketItemDto(int ProductId, int Quantity);
