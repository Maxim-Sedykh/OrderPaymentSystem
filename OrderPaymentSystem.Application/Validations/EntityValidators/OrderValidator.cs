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
    public class OrderValidator : IBaseValidator<Order>
    {
        public BaseResult ValidateOnNull(Order order)
        {
            if (order == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.ProductNotFound,
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }
            return new BaseResult();
        }
    }
}
