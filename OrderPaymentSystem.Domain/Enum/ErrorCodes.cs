namespace OrderPaymentSystem.Domain.Enum;

/// <summary>
/// Перечисление кодов ошибок приложения
/// </summary>
public enum ErrorCodes
{
    #region Product Domain (0-29)
    /// <summary>
    /// Не найдено ни одного товара
    /// </summary>
    ProductsNotFound = 0,

    /// <summary>
    /// Товар не найден
    /// </summary>
    ProductNotFound = 1,

    /// <summary>
    /// Товар с таким именем уже существует
    /// </summary>
    ProductAlreadyExist = 2,
    #endregion

    #region User Domain (30-49)
    /// <summary>
    /// Пользователь не найден
    /// </summary>
    UserNotFound = 30,

    /// <summary>
    /// Пользователь с таким логином уже существует
    /// </summary>
    UserAlreadyExist = 31,

    /// <summary>
    /// Неавторизованный доступ
    /// </summary>
    UserUnauthorizedAccess = 32,

    /// <summary>
    /// У пользователя уже есть эта роль
    /// </summary>
    UserAlreadyExistThisRole = 33,
    #endregion

    #region Authentication Domain (40-49)
    /// <summary>
    /// Пароль и подтверждение пароля не совпадают
    /// </summary>
    PasswordNotEqualsPasswordConfirm = 40,

    /// <summary>
    /// Неверный пароль
    /// </summary>
    PasswordIsWrong = 41,
    #endregion

    #region Role Domain (50-59)
    /// <summary>
    /// Роль не найдена
    /// </summary>
    RoleNotFound = 50,

    /// <summary>
    /// Роль с таким именем уже существует
    /// </summary>
    RoleAlreadyExist = 51,

    /// <summary>
    /// У пользователя не найдены роли
    /// </summary>
    UserRolesNotFound = 52,

    /// <summary>
    /// Не найдено ни одной роли
    /// </summary>
    RolesNotFound = 53,
    #endregion

    #region Order Domain (60-69)
    /// <summary>
    /// Заказ не найден
    /// </summary>
    OrderNotFound = 60,

    /// <summary>
    /// Заказы не найдены
    /// </summary>
    OrdersNotFound = 61,
    #endregion

    #region Basket Domain (70-79)

    /// <summary>
    /// Корзина не найдена
    /// </summary>
    BasketNotFound = 70,

    #endregion

    #region Payment Domain (80-89)

    /// <summary>
    /// Платеж не найден
    /// </summary>
    PaymentNotFound = 80,

    /// <summary>
    /// Платежи не найдены
    /// </summary>
    PaymentsNotFound = 81,

    /// <summary>
    /// Недостаточно средств для оплаты
    /// </summary>
    NotEnoughPayFunds = 82,

    #endregion

    #region Common Domain (90-99)

    /// <summary>
    /// Изменения не обнаружены
    /// </summary>
    NoChangesFound = 90,

    /// <summary>
    /// Неверный запрос клиента
    /// </summary>
    InvalidClientRequest = 95,

    /// <summary>
    /// Невалидный токен
    /// </summary>
    InvalidToken = 96,

    /// <summary>
    /// Срок действия refresh token истек
    /// </summary>
    RefreshTokenExpired = 97,

    #endregion

    #region System Errors (500+)

    /// <summary>
    /// Внутренняя ошибка сервера
    /// </summary>
    InternalServerError = 500,

    #endregion

        None = 0, // No error
    ValidationError = 1000, // General validation error

    // Address related errors (1100-1199)
    AddressStreetEmpty = 1101,
    AddressCityEmpty = 1102,
    AddressPostalCodeEmpty = 1103,
    AddressCountryEmpty = 1104,

    // Product related errors (1200-1299)
    ProductNameEmpty = 1201,
    ProductPriceInvalid = 1202,

    // BasketItem related errors (1300-1399)
    BasketItemQuantityInvalid = 1301,
    BasketItemProductNotFound = 1302, // E.g., when updating quantity of non-existent item

    // User related errors (1400-1499)
    UserLoginEmpty = 1401,
    UserPasswordHashEmpty = 1402,
    UserBasketEmpty = 1404,

    // UserToken related errors (1500-1599)
    RefreshTokenEmpty = 1501,

    // Order related errors (1600-1699)
    OrderDeliveryAddressNull = 1601,
    OrderNoItems = 1602,
    OrderTotalAmountInvalid = 1603,
    OrderProductPriceInvalid = 1604,
    OrderPaymentAlreadyAssigned = 1605,
    OrderCannotChangeDeliveredStatus = 1606,
    OrderCannotChangeCancelledStatus = 1607,
    OrderAlreadyCancelled = 1608,
    OrderCannotCancelShippedOrDelivered = 1609,

    // Payment related errors (1700-1799)
    PaymentAmountDueInvalid = 1701,
    PaymentNotInPendingState = 1702,
    PaymentAmountPaidInvalid = 1703,
    PaymentAmountPaidLessThanDue = 1704,
    PaymentCannotRefundNotSuccessful = 1705,
    PaymentAmountToRefundInvalid = 1706,
    PaymentAmountToRefundGreaterThanPaid = 1707,
}