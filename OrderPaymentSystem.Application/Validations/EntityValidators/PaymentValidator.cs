using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations.EntityValidators
{
    public class PaymentValidator : IPaymentValidator
    {
        public BaseResult PaymentAmountVlidator(decimal costOfBasketOrders, decimal AmountOfPayment)
        {
            if (costOfBasketOrders > AmountOfPayment)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.NotEnoughPayFunds,
                    ErrorCode = (int)ErrorCodes.NotEnoughPayFunds
                };
            }
            return new BaseResult();
        }

        public BaseResult ValidateOnNull(Payment payment)
        {
            if (payment == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.PaymentNotFound,
                    ErrorCode = (int)ErrorCodes.PaymentNotFound
                };
            }
            return new BaseResult();
        }
    }
}
