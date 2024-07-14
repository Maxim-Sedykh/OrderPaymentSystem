using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Token;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Helpers;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Validations.Validators
{
    public class AuthValidator : IAuthValidator
    {
        public BaseResult ValidateLogin(User user, string enteredPassword)
        {
            if (user == null)
            {
                return new BaseResult()
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound
                };
            }

            if (user.Password == HashPasswordHelper.HashPassword(enteredPassword))
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessage.PasswordIsWrong,
                    ErrorCode = (int)ErrorCodes.PasswordIsWrong,
                };
            }

            return new BaseResult();
        }
    }
}
