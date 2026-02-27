namespace OrderPaymentSystem.Domain.Constants;

/// <summary>
/// Определяет числовые коды ошибок, используемые в приложении.
/// Коды сгруппированы по смысловым областям для удобства навигации и понимания.
/// </summary>
public static class ErrorCodes
{
    #region Общие ошибки (Common Errors)

    /// <summary>
    /// Обозначает отсутствие ошибки или успешное выполнение.
    /// </summary>
    public const int None = 0;

    /// <summary>
    /// Запрос клиента некорректен (например, неверный формат данных, отсутствуют обязательные поля).
    /// </summary>
    public const int BadRequest = 400;

    /// <summary>
    /// Запрос требует аутентификации; пользователь не авторизован.
    /// </summary>
    public const int Unauthorized = 401;

    /// <summary>
    /// У пользователя недостаточно прав для выполнения запрошенного действия.
    /// </summary>
    public const int Forbidden = 403;

    /// <summary>
    /// Запрошенный ресурс не найден.
    /// </summary>
    public const int NotFound = 404;

    /// <summary>
    /// Возникает при конфликте состояний; например, попытка создать ресурс, который уже существует.
    /// </summary>
    public const int Conflict = 409;

    /// <summary>
    /// Конфликт данных, связанный с одновременным изменением одного и того же ресурса несколькими пользователями.
    /// </summary>
    public const int ConcurrencyConflict = 409;

    /// <summary>
    /// Непредвиденная ошибка на сервере, не связанная с некорректным запросом клиента.
    /// </summary>
    public const int InternalServerError = 500;

    /// <summary>
    /// Указано, что изменения не были найдены или не были применены.
    /// </summary>
    public const int NoChangesFound = 304;

    /// <summary>
    /// Необходимость положительного значения для количества.
    /// </summary>
    public const int QuantityPositive = 305;

    #endregion

    #region Аутентификация, авторизация, роли, пользователи (Authentication, Authorization, Roles, Users)

    /// <summary>
    /// Некорректный запрос от клиента, связанный с аутентификацией/авторизацией.
    /// </summary>
    public const int InvalidClientRequest = 1012; // Примечание: Код 1012 дублируется с RoleAlreadyExist. Стоит исправить.

    /// <summary>
    /// Пользователь не найден по указанному идентификатору.
    /// </summary>
    public const int UserNotFoundById = 1001;

    /// <summary>
    /// Пользователь с такими учетными данными уже существует.
    /// </summary>
    public const int UserAlreadyExist = 1002;

    /// <summary>
    /// Введенный пароль неверен.
    /// </summary>
    public const int InvalidPassword = 1003;

    /// <summary>
    /// Неверные логин или пароль при попытке входа.
    /// </summary>
    public const int InvalidLoginOrPassword = 1004;

    /// <summary>
    /// Предоставленный токен аутентификации недействителен.
    /// </summary>
    public const int InvalidToken = 1005;

    /// <summary>
    /// Токен обновления (refresh token) истек.
    /// </summary>
    public const int RefreshTokenExpired = 1006;

    /// <summary>
    /// Введенный пароль не совпадает с подтверждением пароля.
    /// </summary>
    public const int PasswordNotEqualsPasswordConfirm = 1007;

    /// <summary>
    /// Роль с указанным именем не найдена.
    /// </summary>
    public const int RoleNotFoundByName = 1008;

    /// <summary>
    /// Указанные роли для пользователя не найдены.
    /// </summary>
    public const int UserRolesNotFound = 1009;

    /// <summary>
    /// У пользователя уже есть указанная роль.
    /// </summary>
    public const int UserAlreadyExistThisRole = 1010;

    /// <summary>
    /// У пользователя отсутствует указанная роль.
    /// </summary>
    public const int UserDoesNotHaveThisRole = 1011;

    /// <summary>
    /// Роль с таким именем уже существует.
    /// </summary>
    public const int RoleAlreadyExist = 1012; // Примечание: Дублирует InvalidClientRequest.

    /// <summary>
    /// Роли с указанными именами не найдены.
    /// </summary>
    public const int RolesNotFoundByName = 1013;

    /// <summary>
    /// Имя роли не может быть пустым.
    /// </summary>
    public const int RoleNameCannotByEmpty = 1014; // Примечание: Этот код дублируется ниже под другим именем.

    /// <summary>
    /// Токен обновления (refresh token) находится в будущем (недействителен).
    /// </summary>
    public const int RefreshTokenFuture = 1018;

    /// <summary>
    /// Хеш пароля не может быть пустым.
    /// </summary>
    public const int PasswordHashCannotBeEmpty = 1020;

    /// <summary>
    /// Имя роли не может быть пустым (повтор).
    /// </summary>
    public const int RoleNameCannotBeEmpty_Duplicate = 1021; // Примечание: Дублирует 1014.

    /// <summary>
    /// Роль с указанным идентификатором не найдена.
    /// </summary>
    public const int RoleNotFoundById = 1022;

    /// <summary>
    /// Роли не найдены.
    /// </summary>
    public const int RolesNotFound = 1023;

    /// <summary>
    /// Пользователь не найден по указанному логину.
    /// </summary>
    public const int UserNotFoundByLogin = 1024;

    /// <summary>
    /// Предоставленные учетные данные (логин/пароль) неверны.
    /// </summary>
    public const int InvalidCredentials = 1025;

    /// <summary>
    /// Связь между пользователем и ролью не найдена.
    /// </summary>
    public const int UserRoleNotFound = 1026;

    /// <summary>
    /// Выбрана та же роль, что уже назначена или была выбрана ранее.
    /// </summary>
    public const int SameRoleSelected = 1027;

    /// <summary>
    /// Объект пользователя оказался пустым (null).
    /// </summary>
    public const int UserWasNull = 1028;

    #endregion

    #region Ошибки товаров (Product Errors)

    /// <summary>
    /// Товар с указанным идентификатором не найден.
    /// </summary>
    public const int ProductNotFound = 2001;

    /// <summary>
    /// Товары не найдены (множественное число).
    /// </summary>
    public const int ProductsNotFound = 2005;

    /// <summary>
    /// Товар с такими параметрами уже существует.
    /// </summary>
    public const int ProductAlreadyExist = 2002;

    /// <summary>
    /// Цена товара должна быть положительным числом.
    /// </summary>
    public const int ProductPriceMustBePositive = 2003;

    /// <summary>
    /// Недоступное количество товара на складе.
    /// </summary>
    public const int ProductStockQuantityNotAvailable = 2004;

    /// <summary>
    /// Цена товара должна быть положительным числом (повтор).
    /// </summary>
    public const int ProductPricePositive = 2006; // Примечание: Дублирует 2003.

    /// <summary>
    /// Количество товара на складе должно быть положительным (повтор).
    /// </summary>
    public const int StockQuantityPositive = 2006; // Примечание: Дублирует 2004.

    #endregion

    #region Ошибки элементов корзин (Basket Item Errors)

    /// <summary>
    /// Элемент корзины с указанным идентификатором не найден.
    /// </summary>
    public const int BasketItemNotFound = 3001;

    /// <summary>
    /// Количество элемента корзины должно быть положительным.
    /// </summary>
    public const int BasketItemQuantityMustBePositive = 3002;

    /// <summary>
    /// Невозможно увеличить количество выше установленного лимита.
    /// </summary>
    public const int CannotIncreaseQuantityPastLimit = 3003;

    /// <summary>
    /// Количество не может быть равно нулю или отрицательным.
    /// </summary>
    public const int QuantityCannotBeZeroOrNegative = 3004;

    /// <summary>
    /// Количество элемента корзины не может быть уменьшено до нуля или менее.
    /// </summary>
    public const int BasketItemCannotDecreaseBelowOne = 3005;

    #endregion

    #region Ошибки заказов (Order Errors)

    /// <summary>
    /// Заказ с указанным идентификатором не найден.
    /// </summary>
    public const int OrderNotFound = 4001;

    /// <summary>
    /// Заказы не найдены (множественное число).
    /// </summary>
    public const int OrdersNotFound = 4002;

    /// <summary>
    /// Требуется указать адрес доставки для заказа.
    /// </summary>
    public const int OrderDeliveryAddressRequired = 4003;

    /// <summary>
    /// Заказ должен содержать хотя бы один элемент.
    /// </summary>
    public const int OrderMustContainAtLeastOneItem = 4004;

    /// <summary>
    /// Общая сумма заказа должна быть положительной.
    /// </summary>
    public const int OrderTotalAmountMustBePositive = 4005;

    /// <summary>
    /// Изменение статуса заказа недопустимо в текущем состоянии.
    /// </summary>
    public const int OrderStatusChangeNotAllowed = 4006;

    /// <summary>
    /// Платеж уже назначен для этого заказа.
    /// </summary>
    public const int OrderPaymentAlreadyAssigned = 4007;

    /// <summary>
    /// Заказ не может быть отменен, если он уже отправлен или доставлен.
    /// </summary>
    public const int OrderCannotCancelShippedOrDelivered = 4008;

    /// <summary>
    /// Заказ уже был отменен.
    /// </summary>
    public const int OrderAlreadyCancelled = 4009;

    /// <summary>
    /// Нельзя подтвердить пустой заказ.
    /// </summary>
    public const int OrderCannotConfirmEmptyOrder = 4010;

    /// <summary>
    /// Заказ уже подтвержден.
    /// </summary>
    public const int OrderAlreadyConfirmed = 4011;

    /// <summary>
    /// Недопустимо добавлять или удалять элементы в текущем статусе заказа.
    /// </summary>
    public const int OrderCannotAddOrRemoveItemInCurrentStatus = 4012;

    /// <summary>
    /// Нельзя удалить элемент из пустого заказа.
    /// </summary>
    public const int OrderCannotRemoveItemFromEmptyOrder = 4013;

    /// <summary>
    /// Нельзя удалить несуществующий элемент из заказа.
    /// </summary>
    public const int OrderCannotRemoveNonExistingItem = 4014;

    /// <summary>
    /// Нельзя подтвердить заказ без соответствующего платежа.
    /// </summary>
    public const int OrderCannotBeConfirmedWithoutPayment = 4015;

    /// <summary>
    /// Нельзя отправить заказ без соответствующего платежа.
    /// </summary>
    public const int OrderCannotBeShippedWithoutPayment = 4016;

    /// <summary>
    /// Недопустимо изменять статус отмененного заказа.
    /// </summary>
    public const int OrderCannotChangeStatusOfACancelledOrder = 4017;

    /// <summary>
    /// Недопустимый статус для подтверждения заказа.
    /// </summary>
    public const int OrderCannotBeConfirmedInvalidStatus = 4018;

    #endregion

    #region Ошибки элементов заказа (Order Item Errors)

    /// <summary>
    /// Элемент заказа с указанным идентификатором не найден.
    /// </summary>
    public const int OrderItemNotFound = 5001;

    /// <summary>
    /// Недопустимое количество для элемента заказа.
    /// </summary>
    public const int OrderItemQuantityInvalid = 5002;

    /// <summary>
    /// Недопустимая цена для элемента заказа.
    /// </summary>
    public const int OrderItemPriceInvalid = 5003;

    /// <summary>
    /// Элементы заказа не найдены (множественное число).
    /// </summary>
    public const int OrderItemsNotFound = 5004;

    #endregion

    #region Ошибки платежей (Payment Errors)

    /// <summary>
    /// Платеж с указанным идентификатором не найден.
    /// </summary>
    public const int PaymentNotFound = 6001;

    /// <summary>
    /// Платеж уже существует для данного заказа.
    /// </summary>
    public const int PaymentAlreadyExistsForOrder = 6002;

    /// <summary>
    /// Сумма платежа к оплате должна быть положительной.
    /// </summary>
    public const int PaymentAmountToPayMustBePositive = 6003;

    /// <summary>
    /// Изменение статуса платежа недопустимо в текущем состоянии.
    /// </summary>
    public const int PaymentStatusChangeNotAllowed = 6004;

    /// <summary>
    /// Платеж уже обработан.
    /// </summary>
    public const int PaymentAlreadyProcessed = 6005;

    /// <summary>
    /// Сумма платежа не соответствует ожидаемой.
    /// </summary>
    public const int PaymentAmountMismatched = 6006;

    /// <summary>
    /// Платеж не связан с заказом.
    /// </summary>
    public const int PaymentOrderNotAssociated = 6007;

    /// <summary>
    /// Недостаточно средств для проведения платежа.
    /// </summary>
    public const int PaymentInsufficientFunds = 6008;

    /// <summary>
    /// Сумма платежа должна быть положительной (повтор).
    /// </summary>
    public const int PaymentAmountPositive = 6010; // Примечание: Дублирует 6003.

    /// <summary>
    /// Недопустимый статус платежа.
    /// </summary>
    public const int PaymentInvalidStatus = 6011;

    /// <summary>
    /// Суммы платежа недостаточно.
    /// </summary>
    public const int PaymentNotEnoughAmount = 6012;

    /// <summary>
    /// Несоответствие при расчете сдачи для платежа.
    /// </summary>
    public const int PaymentCashChangeMismatch = 6013;

    #endregion

    #region Валидация (Validation)

    /// <summary>
    /// Обязательное поле не заполнено.
    /// </summary>
    public const int FieldRequired = 8001;

    /// <summary>
    /// Значение поля превышает максимально допустимую длину.
    /// </summary>
    public const int FieldValueTooLong = 8002;

    /// <summary>
    /// Значение поля короче минимально допустимой длины.
    /// </summary>
    public const int FieldValueTooShort = 8003;

    /// <summary>
    /// Значение поля имеет неверный формат.
    /// </summary>
    public const int InvalidFieldFormat = 8004;

    #endregion
}
