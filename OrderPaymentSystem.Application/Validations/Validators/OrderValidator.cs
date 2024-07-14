using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations.Validators
{
    public class OrderValidator: IOrderValidator
    {
        public BaseResult ValidateUpdatingOrder(Order order, Product product)
        {
            if (order == null)
            {
                return new BaseResult()
                {
                    ErrorCode = (int)ErrorCodes.OrderNotFound,
                    ErrorMessage = ErrorMessage.OrderNotFound
                };
            }

            if (product == null)
            {
                return new BaseResult()
                {
                    ErrorCode = (int)ErrorCodes.ProductNotFound,
                    ErrorMessage = ErrorMessage.ProductNotFound
                };
            }

            return new BaseResult();
        }

        public BaseResult ValidateCreatingOrder(User user, Product product)
        {
            if (user == null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound
                };
            }

            if (product == null)
            {
                return new BaseResult()
                {
                    ErrorCode = (int)ErrorCodes.ProductNotFound,
                    ErrorMessage = ErrorMessage.ProductNotFound
                };
            }

            return new BaseResult();
        }
    }
}
