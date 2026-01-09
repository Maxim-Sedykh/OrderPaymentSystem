using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Validations.Validators;

public class PaymentValidator : IPaymentValidator
{
    public BaseResult ValidateCreatingPayment(bool orderExists, bool paymentExists, long orderId)
    {
        if (!orderExists)
        {
            return BaseResult.Failure(ErrorCodes.OrderNotFound,
                string.Format(ErrorMessage.OrderNotFound, orderId));
        }

        if (paymentExists)
        {
            return BaseResult.Failure(ErrorCodes.PaymentAlreadyExistsForOrder,
                string.Format(ErrorMessage.PaymentAlreadyExistsForOrder, orderId));
        }

        return BaseResult.Success();
    }
}
