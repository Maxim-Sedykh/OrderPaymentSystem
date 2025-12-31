namespace OrderPaymentSystem.Domain.Enum;

/// <summary>
/// Перечисление кодов ошибок приложения
/// </summary>
public enum ErrorCodes
{
    // Общие ошибки
    BadRequest = 400,
    NotFound = 404,
    InternalServerError = 500,
    Unauthorized = 401,
    Forbidden = 403,
    Conflict = 409,
    NoChangesFound = 304, // Хотя это не HTTP-код, но для Result Pattern полезно

    // Ошибки Аутентификации и Авторизации (1xxx)
    UserNotFound = 1001,
    UserAlreadyExist = 1002,
    InvalidPassword = 1003,
    InvalidLoginOrPassword = 1004, // Объединил InvalidLogin и InvalidPassword для Login-а
    InvalidToken = 1005,
    RefreshTokenExpired = 1006,
    PasswordNotEqualsPasswordConfirm = 1007,
    RoleNotFound = 1008,
    UserRolesNotFound = 1009,
    UserAlreadyHasThisRole = 1010,
    UserDoesNotHaveThisRole = 1011, // Для удаления/изменения роли
    InvalidClientRequest = 1012, // Общие ошибки запроса клиента

    // Ошибки Продукта (2xxx)
    ProductNotFound = 2001,
    ProductAlreadyExist = 2002,
    ProductPriceMustBePositive = 2003, // Из домена
    ProductStockQuantityNotAvailable = 2004, // Из домена (проверка на складе)
    ProductsNotFound = 2005, // Для коллекций

    // Ошибки Позиции Корзины (3xxx)
    BasketItemNotFound = 3001,
    BasketItemQuantityMustBePositive = 3002, // Из домена
    CannotIncreaseQuantityPastLimit = 3003, // Из домена
    QuantityCannotBeZeroOrNegative = 3004, // Из домена
    BasketItemCannotDecreaseBelowOne = 3005, // Из домена

    // Ошибки Заказа (4xxx)
    OrderNotFound = 4001,
    OrdersNotFound = 4002, // Для коллекций
    OrderDeliveryAddressRequired = 4003, // Из домена
    OrderMustContainAtLeastOneItem = 4004, // Из домена
    OrderTotalAmountMustBePositive = 4005, // Из домена
    OrderStatusChangeNotAllowed = 4006, // Из домена
    OrderPaymentAlreadyAssigned = 4007, // Из домена
    OrderCannotCancelShippedOrDelivered = 4008, // Из домена
    OrderAlreadyCancelled = 4009, // Из домена
    OrderCannotConfirmEmptyOrder = 4010, // Из домена
    OrderAlreadyConfirmed = 4011, // Из домена
    OrderCannotAddOrRemoveItemInCurrentStatus = 4012, // Из домена
    OrderCannotRemoveItemFromEmptyOrder = 4013, // Из домена
    OrderCannotRemoveNonExistingItem = 4014, // Из домена
    OrderCannotBeConfirmedWithoutPayment = 4015, // Из домена
    OrderCannotBeInStatusWhenDelivered = 4016, // Из домена
    OrderCannotChangeStatusOfACancelledOrder = 4017, // Из домена

    // Ошибки Позиции Заказа (5xxx)
    OrderItemNotFound = 5001,
    OrderItemQuantityInvalid = 5002, // Из домена
    OrderItemPriceInvalid = 5003, // Из домена
    OrderItemsNotFound = 5004, // Для коллекций

    // Ошибки Платежа (6xxx)
    PaymentNotFound = 6001,
    PaymentAlreadyExistsForOrder = 6002,
    PaymentAmountToPayMustBePositive = 6003, // Из домена
    PaymentStatusChangeNotAllowed = 6004, // Из домена
    PaymentAlreadyProcessed = 6005, // Из домена
    PaymentAmountMismatched = 6006, // Из домена
    PaymentOrderNotAssociated = 6007, // Из домена
    PaymentInsufficientFunds = 6008, // Из домена

    RoleAlreadyExist = 7001,
    RolesNotFound = 7002,
    UserAlreadyExistThisRole = 7002,

    QuantityMustBePositive = 10001,
    InvalidUserId = 10002,
    InvalidProductId = 10004,

    InvalidPaymentId = 100077,

    ConcurrencyConflict = 0325493250,
}