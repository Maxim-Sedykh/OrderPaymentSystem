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
    public class RoleValidator : IRoleValidator
    {
        public BaseResult CreateRoleValidator(Role role)
        {
            if (role != null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.RoleAlreadyExist,
                    ErrorCode = (int)ErrorCodes.RoleAlreadyExist
                };
            }

            return new BaseResult();
        }

        public BaseResult ValidateOnNull(Role model)
        {
            if (model == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }
            return new BaseResult();
        }
    }
}
