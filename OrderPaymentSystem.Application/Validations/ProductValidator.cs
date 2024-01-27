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

namespace OrderPaymentSystem.Application.Validations
{
    public class ProductValidator : IProductValidator
    {
        public BaseResult CreateValidator(Product product)
        {
            if (product != null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.ProductAlreadyExist,
                    ErrorCode = (int)ErrorCodes.ProductAlreadyExist
                };
            }

            return new BaseResult();
        }

        public BaseResult ValidateOnNull(Product model)
        {
            if (model == null)
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
