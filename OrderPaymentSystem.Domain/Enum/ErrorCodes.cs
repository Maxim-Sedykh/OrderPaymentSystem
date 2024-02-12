using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Enum
{
    public enum ErrorCodes
    {
        ProductsNotFound = 0,
        ProductNotFound = 1,
        ProductAlreadyExist = 2,

        UserNotFound = 31,
        UserAlreadyExist = 32,
        UserUnauthorizedAccess = 33,

        PasswordNotEqualsPasswordConfirm = 41,
        PasswordIsWrong = 42,



        InternalServerError = 500
    }
}
