using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Constants
{
    public static class CacheKeys
    {
        public const string Product = "product:{0}";
        public const string Products = "products";
        public const string Order = "order:{0}";
        public const string Payment = "payment:{0}";
        public const string UserPayments = "userPayments";
        public const string Roles = "roles";
    }
}
