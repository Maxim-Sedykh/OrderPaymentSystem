using OrderPaymentSystem.Application.Resources;
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
    public class RoleValidator : IRoleValidator
    {
        public BaseResult ValidateRoleForUser(User user, params Role[] roles)
        {
            if (user == null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound
                };
            }

            foreach (Role role in roles)
            {
                if (role == null)
                {
                    return new BaseResult<UserRoleDto>()
                    {
                        ErrorCode = (int)ErrorCodes.RoleNotFound,
                        ErrorMessage = ErrorMessage.RoleNotFound
                    };
                }
            }

            return new BaseResult();
        }
    }
}
