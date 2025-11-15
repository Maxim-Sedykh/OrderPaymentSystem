namespace OrderPaymentSystem.Domain.Enum;

public enum ErrorCodes
{
    ProductsNotFound = 0,
    ProductNotFound = 1,
    ProductAlreadyExist = 2,

    UserNotFound = 31,
    UserAlreadyExist = 32,
    UserUnauthorizedAccess = 33,
    UserAlreadyExistThisRole = 34,

    PasswordNotEqualsPasswordConfirm = 41,
    PasswordIsWrong = 42,

    RoleNotFound = 51,
    RoleAlreadyExist = 52,
    UserRolesNotFound = 53,
    RolesNotFound = 54,

    OrderNotFound = 61,
    OrdersNotFound = 62,

    BasketNotFound = 71,

    PaymentNotFound = 81,
    PaymentsNotFound = 82,
    NotEnoughPayFunds = 83,

    NoChangesFound = 411,

    InternalServerError = 500
}
