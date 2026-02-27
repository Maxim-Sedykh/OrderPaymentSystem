using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Domain.Errors;

/// <summary>
/// Предоставляет статические методы для создания типизированных объектов ошибок (Error),
/// сгруппированных по доменным сущностям или общим категориям.
/// </summary>
public static class DomainErrors
{
    /// <summary>
    /// Предоставляет общие ошибки, не привязанные к конкретной доменной сущности.
    /// </summary>
    public static class General
    {
        /// <summary>
        /// Создает ошибку, указывающую на то, что требуемое количество должно быть положительным.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error QuantityPositive()
            => new(ErrorCodes.QuantityPositive, ErrorMessage.QuantityPositive);

        /// <summary>
        /// Создает ошибку, указывающую на непредвиденную внутреннюю ошибку сервера.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error InternalServerError()
            => new(ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);

        /// <summary>
        /// Создает ошибку, указывающую на отсутствие изменений для сохранения или обработки.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error NoChanges()
            => new(ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);

        /// <summary>
        /// Создает ошибку, указывающую на недействительный токен.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error InvalidToken()
            => new(ErrorCodes.InvalidToken, ErrorMessage.InvalidToken);

        /// <summary>
        /// Создает ошибку, указывающую на некорректный запрос от клиента.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error InvalidClientRequest()
            => new(ErrorCodes.InvalidClientRequest, ErrorMessage.InvalidClientRequest);

        /// <summary>
        /// Создает ошибку, указывающую на конфликт параллельного изменения данных.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error ConcurrencyConflict()
            => new(ErrorCodes.ConcurrencyConflict, ErrorMessage.ConcurrencyConflict);
    }

    /// <summary>
    /// Предоставляет ошибки, связанные с пользователями и их учетными данными.
    /// </summary>
    public static class User
    {
        /// <summary>
        /// Создает ошибку о неверных учетных данных (логин или пароль).
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error InvalidCredentials()
            => new(ErrorCodes.InvalidCredentials, ErrorMessage.InvalidCredentials);

        /// <summary>
        /// Создает ошибку, указывающую, что пользователь с указанным идентификатором не найден.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFoundById(Guid id)
            => new(ErrorCodes.UserNotFoundById, string.Format(ErrorMessage.UserNotFoundById, id));

        /// <summary>
        /// Создает ошибку, указывающую, что пользователь с указанным логином не найден.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFoundByLogin(string login)
            => new(ErrorCodes.UserNotFoundByLogin, string.Format(ErrorMessage.UserNotFoundByLogin, login));

        /// <summary>
        /// Создает ошибку, указывающую, что объект пользователя является null.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error WasNull()
            => new(ErrorCodes.UserWasNull, ErrorMessage.UserNotFoundByLogin); // Примечание: Message.UserNotFoundByLogin может быть некорректным здесь.

        /// <summary>
        /// Создает ошибку, указывающую, что пользователь с таким логином уже существует.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error AlreadyExist(string login)
            => new(ErrorCodes.UserAlreadyExist, string.Format(ErrorMessage.UserAlreadyExist, login));

        /// <summary>
        /// Создает ошибку о несоответствии пароля и подтверждения пароля.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error PasswordMismatch()
            => new(ErrorCodes.PasswordNotEqualsPasswordConfirm, ErrorMessage.PasswordNotEqualsPasswordConfirm);
    }

    /// <summary>
    /// Предоставляет ошибки, связанные с ролями пользователей.
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// Создает ошибку, указывающую, что роль с указанным именем уже существует.
        /// </summary>
        /// <param name="name">Имя роли.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error AlreadyExists(string name)
            => new(ErrorCodes.RoleAlreadyExist, string.Format(ErrorMessage.RoleAlreadyExist, name));

        /// <summary>
        /// Создает ошибку, указывающую, что роль с указанным именем не найдена.
        /// </summary>
        /// <param name="name">Имя роли.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFoundByName(string name)
            => new(ErrorCodes.RoleNotFoundByName, string.Format(ErrorMessage.RoleNotFoundByName, name));

        /// <summary>
        /// Создает ошибку, указывающую, что роль с указанным идентификатором не найдена.
        /// </summary>
        /// <param name="id">Идентификатор роли.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFoundById(int id)
            => new(ErrorCodes.RoleNotFoundById, string.Format(ErrorMessage.RoleNotFoundById, id));

        /// <summary>
        /// Создает ошибку, указывающую, что роли для указанного пользователя не найдены.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFoundByUser(Guid id)
            => new(ErrorCodes.UserRolesNotFound, string.Format(ErrorMessage.UserRolesNotFound, id));

        /// <summary>
        /// Создает ошибку, указывающую, что роли не найдены (общий случай).
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error RolesNotFound()
            => new(ErrorCodes.RolesNotFound, ErrorMessage.RolesNotFound);

        /// <summary>
        /// Создает ошибку, указывающую, что пользователь уже имеет указанную роль.
        /// </summary>
        /// <param name="roleName">Имя роли.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error UserAlreadyHasRole(string roleName)
            => new(ErrorCodes.UserAlreadyExistThisRole,
                string.Format(ErrorMessage.UserAlreadyExistThisRole, roleName));

        /// <summary>
        /// Создает ошибку, указывающую, что связь пользователя с ролью не найдена.
        /// </summary>
        /// <param name="roleId">Идентификатор роли.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error UserRoleNotFound(int roleId)
            => new(ErrorCodes.UserRoleNotFound, string.Format(ErrorMessage.UserRolesNotFound, roleId));

        /// <summary>
        /// Создает ошибку, указывающую, что выбрана уже назначенная роль.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error SameRoleSelected()
            => new(ErrorCodes.SameRoleSelected, ErrorMessage.SameRoleSelected);
    }

    /// <summary>
    /// Предоставляет ошибки, связанные с товарами.
    /// </summary>
    public static class Product
    {
        /// <summary>
        /// Создает ошибку, указывающую, что товар с указанным идентификатором не найден.
        /// </summary>
        /// <param name="id">Идентификатор товара.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFound(int id)
            => new(ErrorCodes.ProductNotFound, string.Format(ErrorMessage.ProductNotFound, id));

        /// <summary>
        /// Создает ошибку, указывающую, что товар с таким именем уже существует.
        /// </summary>
        /// <param name="name">Название товара.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error AlreadyExist(string name)
            => new(ErrorCodes.ProductAlreadyExist, string.Format(ErrorMessage.ProductAlreadyExist, name));

        /// <summary>
        /// Создает ошибку, указывающую, что цена товара должна быть положительной.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error PricePositive()
            => new(ErrorCodes.ProductPricePositive, ErrorMessage.ProductPricePositive);

        /// <summary>
        /// Создает ошибку, указывающую на недостаточное количество товара на складе.
        /// </summary>
        /// <param name="requested">Запрошенное количество.</param>
        /// <param name="id">Идентификатор товара.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error StockNotAvailable(int requested, int id)
            => new(ErrorCodes.ProductStockQuantityNotAvailable,
                string.Format(ErrorMessage.ProductStockQuantityNotAvailable, requested, id));

        /// <summary>
        /// Создает ошибку, указывающую, что количество товара на складе должно быть положительным.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error StockPositive()
            => new(ErrorCodes.StockQuantityPositive, ErrorMessage.StockQuantityPositive);
    }

    /// <summary>
    /// Предоставляет ошибки, связанные с элементами корзины.
    /// </summary>
    public static class BasketItem
    {
        /// <summary>
        /// Создает ошибку, указывающую, что элемент корзины с указанным идентификатором не найден.
        /// </summary>
        /// <param name="id">Идентификатор элемента корзины.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFound(long id)
            => new(ErrorCodes.BasketItemNotFound, string.Format(ErrorMessage.BasketItemNotFound, id));

    }

    /// <summary>
    /// Предоставляет ошибки, связанные с заказами.
    /// </summary>
    public static class Order
    {
        /// <summary>
        /// Создает ошибку, указывающую, что заказ с указанным идентификатором не найден.
        /// </summary>
        /// <param name="id">Идентификатор заказа.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFound(long id)
            => new(ErrorCodes.OrderNotFound, string.Format(ErrorMessage.OrderNotFound, id));

        /// <summary>
        /// Создает ошибку, указывающую, что элемент заказа с указанным идентификатором не найден.
        /// </summary>
        /// <param name="itemId">Идентификатор элемента заказа.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error ItemNotFound(long itemId)
            => new(ErrorCodes.OrderItemNotFound, string.Format(ErrorMessage.OrderItemNotFound, itemId));

        /// <summary>
        /// Создает ошибку, указывающую, что заказ не содержит элементов.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error ItemsEmpty()
            => new(ErrorCodes.OrderItemsNotFound, ErrorMessage.OrderItemsEmpty);

        /// <summary>
        /// Создает ошибку, указывающую, что для заказа требуется адрес доставки.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error DeliveryAddressRequired()
            => new(ErrorCodes.OrderDeliveryAddressRequired, ErrorMessage.OrderDeliveryAddressRequired);

        /// <summary>
        /// Создает ошибку, указывающую, что изменение статуса заказа с '{from}' на '{to}' недопустимо.
        /// </summary>
        /// <param name="from">Исходный статус заказа.</param>
        /// <param name="to">Целевой статус заказа.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error StatusChangeNotAllowed(string from, string to)
            => new(ErrorCodes.OrderStatusChangeNotAllowed,
                string.Format(ErrorMessage.OrderStatusChangeNotAllowed, from, to));

        /// <summary>
        /// Создает ошибку, указывающую, что заказ не может быть подтвержден без оплаты.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error CannotBeConfirmedWithoutPayment()
            => new(ErrorCodes.OrderCannotBeConfirmedWithoutPayment,
                string.Format(ErrorMessage.OrderCannotBeConfirmedWithoutPayment));

        /// <summary>
        /// Создает ошибку, указывающую, что заказ не может быть отправлен без оплаты.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error CannotBeShippedWithoutPayment()
            => new(ErrorCodes.OrderCannotBeShippedWithoutPayment,
                string.Format(ErrorMessage.OrderCannotBeShipped));

        /// <summary>
        /// Создает ошибку, указывающую, что добавление или удаление элементов недопустимо в текущем статусе заказа.
        /// </summary>
        /// <param name="status">Текущий статус заказа.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error CannotAddOrRemoveItemInCurrentStatus(OrderStatus status)
            => new(ErrorCodes.OrderCannotAddOrRemoveItemInCurrentStatus,
                string.Format(ErrorMessage.OrderCannotAddOrRemoveItemInCurrentStatus, status));
    }

    /// <summary>
    /// Предоставляет ошибки, связанные с платежами.
    /// </summary>
    public static class Payment
    {
        /// <summary>
        /// Создает ошибку, указывающую, что платеж с указанным идентификатором не найден.
        /// </summary>
        /// <param name="id">Идентификатор платежа.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotFound(long id)
            => new(ErrorCodes.PaymentNotFound, string.Format(ErrorMessage.PaymentNotFound, id));

        /// <summary>
        /// Создает ошибку, указывающую, что для заказа с ID '{orderId}' уже существует платеж.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error AlreadyExists(long orderId)
            => new(ErrorCodes.PaymentAlreadyExistsForOrder, string.Format(ErrorMessage.PaymentAlreadyExistsForOrder, orderId));

        /// <summary>
        /// Создает ошибку, указывающую, что платеж не связан ни с одним заказом.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error OrderNotAssociated()
            => new(ErrorCodes.PaymentOrderNotAssociated, ErrorMessage.PaymentOrderNotAssociated);

        /// <summary>
        /// Создает ошибку, указывающую, что сумма платежа должна быть положительной.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error AmountPositive()
            => new(ErrorCodes.PaymentAmountPositive, ErrorMessage.PaymentAmountPositive);

        /// <summary>
        /// Создает ошибку, указывающую на недопустимое изменение статуса платежа с '{from}' на '{to}'.
        /// </summary>
        /// <param name="from">Исходный статус платежа.</param>
        /// <param name="to">Целевой статус платежа.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error InvalidStatus(string from, string to)
            => new(ErrorCodes.PaymentInvalidStatus, string.Format(ErrorMessage.PaymentInvalidStatus, from, to));

        /// <summary>
        /// Создает ошибку, указывающую на то, что внесенной суммы недостаточно для оплаты.
        /// </summary>
        /// <param name="paid">Внесенная сумма.</param>
        /// <param name="required">Требуемая сумма.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error NotEnoughAmount(decimal paid, decimal required)
            => new(ErrorCodes.PaymentNotEnoughAmount, string.Format(ErrorMessage.PaymentNotEnoughAmount,
                paid,
                required));

        /// <summary>
        /// Создает ошибку, указывающую на несоответствие при расчете сдачи.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error CashChangeMismatch()
            => new(ErrorCodes.PaymentCashChangeMismatch, ErrorMessage.PaymentCashChangeMismatch);
    }

    /// <summary>
    /// Предоставляет ошибки, связанные с токенами аутентификации/обновления.
    /// </summary>
    public static class Token
    {
        /// <summary>
        /// Создает ошибку, указывающую, что токен обновления истек.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error RefreshExpired()
            => new(ErrorCodes.RefreshTokenExpired, ErrorMessage.RefreshTokenExpired);

        /// <summary>
        /// Создает ошибку, указывающую, что токен обновления имеет дату в будущем и недействителен.
        /// </summary>
        /// <returns>Объект ошибки.</returns>
        public static Error RefreshFuture()
            => new(ErrorCodes.RefreshTokenFuture, ErrorMessage.RefreshTokenFuture);
    }

    /// <summary>
    /// Предоставляет общие ошибки валидации полей.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Создает ошибку, указывающую, что поле '{fieldName}' является обязательным.
        /// </summary>
        /// <param name="fieldName">Имя поля.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error Required(string fieldName)
            => new(ErrorCodes.FieldRequired, string.Format(ErrorMessage.FieldRequired, fieldName));

        /// <summary>
        /// Создает ошибку, указывающую, что значение поля '{fieldName}' превышает максимальную длину в {max} символов.
        /// </summary>
        /// <param name="fieldName">Имя поля.</param>
        /// <param name="max">Максимально допустимая длина.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error TooLong(string fieldName, int max)
            => new(ErrorCodes.FieldValueTooLong, string.Format(ErrorMessage.FieldValueToLong, fieldName, max));

        /// <summary>
        /// Создает ошибку, указывающую, что значение поля '{fieldName}' короче минимально допустимой длины в {min} символов.
        /// </summary>
        /// <param name="fieldName">Имя поля.</param>
        /// <param name="min">Минимально допустимая длина.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error TooShort(string fieldName, int min)
            => new(ErrorCodes.FieldValueTooShort, string.Format(ErrorMessage.FieldValueTooShort, fieldName, min));

        /// <summary>
        /// Создает ошибку, указывающую, что значение поля '{fieldName}' имеет неверный формат.
        /// </summary>
        /// <param name="fieldName">Имя поля.</param>
        /// <returns>Объект ошибки.</returns>
        public static Error InvalidFormat(string fieldName)
            => new(ErrorCodes.InvalidFieldFormat, string.Format(ErrorMessage.FieldInvalidFormat, fieldName));
    }
}
