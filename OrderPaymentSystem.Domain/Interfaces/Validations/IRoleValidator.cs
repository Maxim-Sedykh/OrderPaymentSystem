using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Validations
{
    public interface IRoleValidator : IBaseValidator<Role>
    {
        /// <summary>
        /// При создании роли проверяется, есть ли роль с таким же названием
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        BaseResult CreateRoleValidator(Role product);
    }
}
