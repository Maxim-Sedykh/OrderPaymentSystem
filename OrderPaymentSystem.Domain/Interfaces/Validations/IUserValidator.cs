using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Validations
{
    public interface IUserValidator : IBaseValidator<User>
    {
        /// <summary>
        /// Проверяется наличие товара, если товар с переданным названием есть в БД, то сознать точно такой же нельзя
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        BaseResult CreateUserValidate(User user);
    }
}
