namespace OrderPaymentSystem.Application.DTOs.Basket;

/// <summary>
/// Модель данных для отображения информации о элементе корзины
/// </summary>
/// <param name="Id">Id элемента</param>
/// <param name="UserId">Id пользователя которому принадлежит элемент</param>
/// <param name="ProductId">Id товара</param>
/// <param name="Quantity">Количество товара</param>
public record BasketItemDto(long Id, Guid UserId, int ProductId, int Quantity);
