using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Validations.Validators;

/// <summary>
/// Валидатор для процесса работы с заказами
/// </summary>
public class OrderItemValidator : IOrderItemValidator
{
    /// <inheritdoc/>
    public BaseResult ValidateUpdatingOrder(Order order, Product product)
    {
        if (order == null)
        {
            return BaseResult.Failure(ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
        }

        if (product == null)
        {
            return BaseResult.Failure(ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public BaseResult ValidateCreatingOrder(User user, Product product)
    {
        if (user == null)
        {
            return BaseResult.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        if (product == null)
        {
            return BaseResult.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        return BaseResult.Success();
    }
}
