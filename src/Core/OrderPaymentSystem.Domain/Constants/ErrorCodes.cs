namespace OrderPaymentSystem.Domain.Constants;

/// <summary>
/// Числовые коды ошибок приложения
/// </summary>
public static class ErrorCodes
{
    #region Общие ошибки

    public const int None = 0;

    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int Conflict = 409;
    public const int ConcurrencyConflict = 409;
    public const int InternalServerError = 500;
    public const int NoChangesFound = 304;
    public const int QuantityPositive = 305;

    #endregion

    #region Аутентификация авторизация, роли пользователи

    public const int InvalidClientRequest = 1012;

    public const int UserNotFoundById = 1001;
    public const int UserAlreadyExist = 1002;
    public const int InvalidPassword = 1003;
    public const int InvalidLoginOrPassword = 1004;
    public const int InvalidToken = 1005;
    public const int RefreshTokenExpired = 1006;
    public const int PasswordNotEqualsPasswordConfirm = 1007;
    public const int RoleNotFoundByName = 1008;
    public const int UserRolesNotFound = 1009;
    public const int UserAlreadyExistThisRole = 1010;
    public const int UserDoesNotHaveThisRole = 1011;
    public const int RoleAlreadyExist = 1012;
    public const int RolesNotFoundByName = 1013;
    public const int RoleNameCannotByEmpty = 1014;
    public const int RefreshTokenFuture = 1018;
    public const int PasswordHashCannotBeEmpty = 1020;
    public const int RoleNameCannotBeEmpty = 1021;
    public const int RoleNotFoundById = 1022;
    public const int RolesNotFound = 1023;
    public const int UserNotFoundByLogin = 1024;
    public const int InvalidCredentials = 1025;
    public const int UserRoleNotFound = 1026;
    public const int SameRoleSelected = 1027;


    #endregion

    #region Ошибки товаров

    public const int ProductNotFound = 2001;
    public const int ProductsNotFound = 2005;
    public const int ProductAlreadyExist = 2002;
    public const int ProductPriceMustBePositive = 2003;
    public const int ProductStockQuantityNotAvailable = 2004;
    public const int ProductPricePositive = 2006;
    public const int StockQuantityPositive = 2006;

    #endregion

    #region Ошибки элементов корзин

    public const int BasketItemNotFound = 3001;
    public const int BasketItemQuantityMustBePositive = 3002;
    public const int CannotIncreaseQuantityPastLimit = 3003;
    public const int QuantityCannotBeZeroOrNegative = 3004;
    public const int BasketItemCannotDecreaseBelowOne = 3005;

    #endregion

    #region Ошибки заказов

    public const int OrderNotFound = 4001;
    public const int OrdersNotFound = 4002;
    public const int OrderDeliveryAddressRequired = 4003;
    public const int OrderMustContainAtLeastOneItem = 4004;
    public const int OrderTotalAmountMustBePositive = 4005;
    public const int OrderStatusChangeNotAllowed = 4006;
    public const int OrderPaymentAlreadyAssigned = 4007;
    public const int OrderCannotCancelShippedOrDelivered = 4008;
    public const int OrderAlreadyCancelled = 4009;
    public const int OrderCannotConfirmEmptyOrder = 4010;
    public const int OrderAlreadyConfirmed = 4011;
    public const int OrderCannotAddOrRemoveItemInCurrentStatus = 4012;
    public const int OrderCannotRemoveItemFromEmptyOrder = 4013;
    public const int OrderCannotRemoveNonExistingItem = 4014;
    public const int OrderCannotBeConfirmedWithoutPayment = 4015;
    public const int OrderCannotBeShippedWithoutPayment = 4016;
    public const int OrderCannotChangeStatusOfACancelledOrder = 4017;
    public const int OrderCannotBeConfirmedInvalidStatus = 4018;

    #endregion

    #region Ошибки элементов заказа

    public const int OrderItemNotFound = 5001;
    public const int OrderItemQuantityInvalid = 5002;
    public const int OrderItemPriceInvalid = 5003;
    public const int OrderItemsNotFound = 5004;

    #endregion

    #region Ошибки платежей

    public const int PaymentNotFound = 6001;
    public const int PaymentAlreadyExistsForOrder = 6002;
    public const int PaymentAmountToPayMustBePositive = 6003;
    public const int PaymentStatusChangeNotAllowed = 6004;
    public const int PaymentAlreadyProcessed = 6005;
    public const int PaymentAmountMismatched = 6006;
    public const int PaymentOrderNotAssociated = 6007;
    public const int PaymentInsufficientFunds = 6008;
    public const int PaymentAmountPositive = 6010;
    public const int PaymentInvalidStatus = 6011;
    public const int PaymentNotEnoughAmount = 6012;
    public const int PaymentCashChangeMismatch = 6013;

    #endregion

    #region Валидация

    public const int FieldRequired = 8001;
    public const int FieldValueTooLong = 8002;
    public const int FieldValueTooShort = 8003;
    public const int InvalidFieldFormat = 8004;

    #endregion
}
