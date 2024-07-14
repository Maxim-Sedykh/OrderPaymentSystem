using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Validators
{
    public interface IOrderValidator
    {
        BaseResult ValidateUpdatingOrder(Order order, Product product);

        BaseResult ValidateCreatingOrder(User user, Product product);
    }
}
