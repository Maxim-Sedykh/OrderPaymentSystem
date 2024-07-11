namespace OrderPaymentSystem.Domain.Dto.Order
{
    /// <summary>
    /// Модель, предназначенная для методов действий с заказами (создание, удаление)
    /// </summary>
    /// <param name="ProductId"></param>
    /// <param name="ProductCount"></param>
    public record CreateOrderDto
    (
        long UserId,
        int ProductId,
        int ProductCount
    );
}
