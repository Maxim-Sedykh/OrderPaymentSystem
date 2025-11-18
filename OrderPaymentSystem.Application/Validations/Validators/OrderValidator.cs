using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Validations.Validators;

/// <summary>
/// Валидатор для процесса работы с заказами
/// </summary>
public class OrderValidator : IOrderValidator
{
    /// <inheritdoc/>
    public BaseResult ValidateUpdatingOrder(Order order, Product product)
    {
        if (order == null)
        {
            return BaseResult.Failure((int)ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
        }

        if (product == null)
        {
            return BaseResult.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
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
