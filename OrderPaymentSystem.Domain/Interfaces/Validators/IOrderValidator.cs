using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Validators;

/// <summary>
/// Валидатор по взаимодействию с заказами
/// </summary>
public interface IOrderItemValidator
{
    /// <summary>
    /// Валидировать обновление заказа
    /// </summary>
    /// <param name="order">Заказ</param>
    /// <param name="product">Товар</param>
    /// <returns>Результат валидации</returns>
    BaseResult ValidateUpdatingOrder(Order order, Product product);

    /// <summary>
    /// Валидировать создание заказа
    /// </summary>
    /// <param name="user">Пользователь который создаёт заказ</param>
    /// <param name="product">Товар по которому создаётся заказ</param>
    /// <returns>Результат валидации</returns>
    BaseResult ValidateCreatingOrder(User user, Product product);
}
