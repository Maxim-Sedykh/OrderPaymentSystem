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
    public class UserValidator : IUserValidator
    {
        public BaseResult CreateUserValidate(User user)
        {
            if (user != null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.UserAlreadyExist,
                    ErrorCode = (int)ErrorCodes.UserAlreadyExist
                };
            }

            return new BaseResult();
        }

        public BaseResult ValidateOnNull(User user)
        {
            if (user == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }
            return new BaseResult();
        }
    }
}
