using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Shared.Result;
using System;
using System.Collections.Generic;
using System.Text;

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
