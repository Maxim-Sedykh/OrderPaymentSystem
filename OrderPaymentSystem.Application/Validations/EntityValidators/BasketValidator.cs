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
    public class BasketValidator : IBaseValidator<Basket>
    {
        public BaseResult ValidateOnNull(Basket basket)
        {
            if (basket == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.BasketNotFound,
                    ErrorCode = (int)ErrorCodes.BasketNotFound
                };
            }
            return new BaseResult();
        }
    }
}
