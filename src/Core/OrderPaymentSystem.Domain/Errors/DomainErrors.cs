using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Domain.Errors;

public static class DomainErrors
{
    public static class General
    {
        public static Error InternalServerError() 
            => new(ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);

        public static Error NoChanges() 
            => new(ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);

        public static Error InvalidToken() 
            => new(ErrorCodes.InvalidToken, ErrorMessage.InvalidToken);

        public static Error InvalidClientRequest() 
            => new(ErrorCodes.InvalidClientRequest, ErrorMessage.InvalidClientRequest);

        public static Error ConcurrencyConflict() 
            => new(ErrorCodes.ConcurrencyConflict, ErrorMessage.ConcurrencyConflict);
    }

    public static class User
    {
        public static Error NotFoundById(Guid id) 
            => new(ErrorCodes.UserNotFoundById, string.Format(ErrorMessage.UserNotFoundById, id));

        public static Error NotFoundByLogin(string login)
            => new(ErrorCodes.UserNotFoundByLogin, string.Format(ErrorMessage.UserNotFoundByLogin, login));

        public static Error AlreadyExist(string login) 
            => new(ErrorCodes.UserAlreadyExist, string.Format(ErrorMessage.UserAlreadyExist, login));

        public static Error InvalidId() 
            => new(ErrorCodes.InvalidUserId, ErrorMessage.InvalidUserId);

        public static Error PasswordMismatch() 
            => new(ErrorCodes.PasswordNotEqualsPasswordConfirm, ErrorMessage.PasswordNotEqualsPasswordConfirm);

        public static Error LoginEmpty() 
            => new(ErrorCodes.LoginCannotBeEmpty, ErrorMessage.LoginCannotBeEmpty);

        public static Error PasswordHashEmpty() 
            => new(ErrorCodes.PasswordHashCannotBeEmpty, ErrorMessage.PasswordHashCannotBeEmpty);
    }

    public static class Role
    {
        public static Error AlreadyExists(string name)
            => new(ErrorCodes.RoleAlreadyExist, string.Format(ErrorMessage.RoleAlreadyExist, name));

        public static Error NotFoundByName(string name) 
            => new(ErrorCodes.RoleNotFoundByName, string.Format(ErrorMessage.RoleNotFoundByName, name));

        public static Error NotFoundById(int id)
            => new(ErrorCodes.RoleNotFoundById, string.Format(ErrorMessage.RoleNotFoundById, id));

        public static Error NotFoundByUser(Guid id)
            => new(ErrorCodes.UserRolesNotFound, string.Format(ErrorMessage.UserRolesNotFound, id));

        public static Error RolesNotFound() 
            => new(ErrorCodes.RolesNotFound, ErrorMessage.RolesNotFound);

        public static Error NameEmpty() 
            => new(ErrorCodes.RoleNameCannotBeEmpty, ErrorMessage.RoleNameCannotBeEmpty);

        public static Error UserRolesNotFound() 
            => new(ErrorCodes.UserRolesNotFound, ErrorMessage.UserRolesNotFound);

        public static Error UserAlreadyHasRole(int roleId) 
            => new(ErrorCodes.UserAlreadyExistThisRole,
                string.Format(ErrorMessage.UserAlreadyExistThisRole, roleId));
    }

    public static class Product
    {
        public static Error NotFound(int id) 
            => new(ErrorCodes.ProductNotFound, string.Format(ErrorMessage.ProductNotFound, id));

        public static Error AlreadyExist(string name) 
            => new(ErrorCodes.ProductAlreadyExist, string.Format(ErrorMessage.ProductAlreadyExist, name));

        public static Error NameEmpty() 
            => new(ErrorCodes.ProductNameEmpty, ErrorMessage.ProductNameEmpty);

        public static Error PricePositive(decimal price) 
            => new(ErrorCodes.ProductPricePositive, string.Format(ErrorMessage.ProductPricePositive, price));

        public static Error StockNotAvailable(int requested, int id) 
            => new(ErrorCodes.ProductStockQuantityNotAvailable,
                string.Format(ErrorMessage.ProductStockQuantityNotAvailable, requested, id));

        public static Error StockPositive() 
            => new(ErrorCodes.StockQuantityPositive, ErrorMessage.StockQuantityPositive);
    }

    public static class BasketItem
    {
        public static Error NotFound(long id) 
            => new(ErrorCodes.BasketItemNotFound, string.Format(ErrorMessage.BasketItemNotFound, id));
        public static Error QuantityPositive() 
            => new(ErrorCodes.QuantityCannotBeZeroOrNegative, ErrorMessage.QuantityPositive);
    }

    public static class Order
    {
        public static Error NotFound(long id) 
            => new(ErrorCodes.OrderNotFound, string.Format(ErrorMessage.OrderNotFound, id));

        public static Error ItemNotFound(long itemId) 
            => new(ErrorCodes.OrderItemNotFound, string.Format(ErrorMessage.OrderItemNotFound, itemId));

        public static Error ItemsEmpty() 
            => new(ErrorCodes.OrderItemsNotFound, ErrorMessage.OrderItemsEmpty);

        public static Error DeliveryAddressRequired() 
            => new(ErrorCodes.OrderDeliveryAddressRequired, ErrorMessage.OrderDeliveryAddressRequired);

        public static Error StatusChangeNotAllowed(string from, string to) 
            => new(ErrorCodes.OrderCannotBeInStatusWhenDelivered,
                string.Format(ErrorMessage.OrderStatusChangeNotAllowed, from, to));

        public static Error CannotBeShipped(string reason) 
            => new(ErrorCodes.OrderStatusChangeNotAllowed,
                string.Format(ErrorMessage.OrderCannotBeShipped, reason));

        public static Error CannotBeConfirmedWithoutPayment()
            => new(ErrorCodes.OrderCannotBeConfirmedWithoutPayment,
                string.Format(ErrorMessage.OrderCann, reason));

        public static Error CannotBeConfirmed(string currentStatus) 
            => new(ErrorCodes.OrderCannotBeConfirmedInvalidStatus,
                string.Format(ErrorMessage.OrderCannotBeConfirmed, currentStatus));

        public static Error InvalidPaymentId() 
            => new(ErrorCodes.InvalidPaymentId, ErrorMessage.InvalidPaymentId);

        public static Error CannotRemoveNonExistingItem() 
            => new(ErrorCodes.OrderCannotRemoveNonExistingItem,
                ErrorMessage.OrderCannotRemoveNonExistingItem);
    }

    public static class Payment
    {
        public static Error NotFound(long id) 
            => new(ErrorCodes.PaymentNotFound, string.Format(ErrorMessage.PaymentNotFound, id));

        public static Error AlreadyExists() 
            => new(ErrorCodes.PaymentAlreadyExistsForOrder, ErrorMessage.PaymentAlreadyExistsForOrder);

        public static Error OrderNotAssociated() 
            => new(ErrorCodes.PaymentOrderNotAssociated, ErrorMessage.PaymentOrderNotAssociated);

        public static Error AmountPositive() 
            => new(ErrorCodes.PaymentAmountPositive, ErrorMessage.PaymentAmountPositive);

        public static Error InvalidStatus(string status) 
            => new(ErrorCodes.PaymentInvalidStatus, string.Format(ErrorMessage.PaymentInvalidStatus, status));

        public static Error NotEnoughAmount(decimal paid, decimal required) 
            => new(ErrorCodes.PaymentNotEnoughAmount, string.Format(ErrorMessage.PaymentNotEnoughAmount,
                paid,
                required));

        public static Error CashChangeMismatch() 
            => new(ErrorCodes.PaymentCashChangeMismatch, ErrorMessage.PaymentCashChangeMismatch);
    }

    public static class Token
    {
        public static Error RefreshEmpty() 
            => new(ErrorCodes.RefreshTokenEmpty, ErrorMessage.RefreshTokenEmpty);

        public static Error RefreshExpired() 
            => new(ErrorCodes.RefreshTokenExpired, ErrorMessage.RefreshTokenExpired);

        public static Error RefreshFuture() 
            => new(ErrorCodes.RefreshTokenFuture, ErrorMessage.RefreshTokenFuture);
    }

    public static class Address
    {
        public static Error StreetEmpty() 
            => new(ErrorCodes.AddressStreetEmpty, ErrorMessage.AddressStreetEmpty);

        public static Error CityEmpty() 
            => new(ErrorCodes.AddressCityEmpty, ErrorMessage.AddressCityEmpty);

        public static Error ZipCodeEmpty() 
            => new(ErrorCodes.AddressZipCodeEmpty, ErrorMessage.AddressZipCodeEmpty);

        public static Error CountryEmpty() 
            => new(ErrorCodes.AddressCountryEmpty, ErrorMessage.AddressCountryEmpty);
    }
}