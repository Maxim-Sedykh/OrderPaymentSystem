using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Enum
{
    public enum PaymentMethod
    {
        CreditCard = 0,
        DebitCard = 1,
        PayPal = 2,
        ApplePay = 3,
        GooglePay = 4,
        Venmo = 5,
        BankTransfer = 6,
        CashOnDelivery = 7,
        Cryptocurrency = 8,
        GiftCard=9,
    }
}
