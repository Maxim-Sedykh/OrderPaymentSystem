using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Validations
{
    public interface IPaymentValidator: IBaseValidator<Payment>
    {
        BaseResult PaymentAmountVlidator(decimal costOfBasketOrders, decimal AmountOfPayment);
    }
}
