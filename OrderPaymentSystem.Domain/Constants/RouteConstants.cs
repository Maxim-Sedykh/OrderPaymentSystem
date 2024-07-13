namespace OrderPaymentSystem.Domain.Constants
{
    /// <summary>
    /// Класс констант для хранения сегментов роута
    /// </summary>
    public static class RouteConstants
    {
        #region Аутентификация

        public const string Register = "register";

        public const string Login = "login";

        #endregion

        #region Токен

        public const string RefreshToken = "refresh-token";

        #endregion

        #region Роли

        public const string CreateRole = "create-role";

        public const string UpdateRole = "update-role";

        public const string DeleteRoleById = "delete-role/{id}";

        public const string AddRoleForUser = "add-role-for-user";

        public const string DeleteRoleForUser = "delete-role-for-user";

        public const string UpdateRoleForUser = "update-role-for-user";

        public const string GetAllRoles = "get-all-roles";

        #endregion

        #region Товары

        public const string CreateProduct = "create-product";

        public const string DeleteProductById = "delete-product/{id}";

        public const string UpdateProduct = "update-product";

        public const string GetProductById = "get-product/{id}";

        public const string GetProducts = "get-products";

        #endregion

        #region Плажёт

        public const string GetPaymentById = "get-payment/{id}";

        public const string GetUserPaymentsByUserId = "get-user-payments/{userId}";

        public const string DeletePaymentById = "delete-payment/{id}";

        public const string CreatePayment = "create-payment";

        public const string UpdatePayment = "update-payment";

        public const string GetPaymentOrdersByPaymentId = "get-payment-orders/{id}";

        #endregion

        #region Заказ

        public const string GetOrderById = "get-order/{id}";

        public const string GetOrders = "get-orders";

        public const string DeleteOrderById = "delete-order/{id}";

        public const string CreateOrder = "create-order";

        public const string UpdateOrder = "update-order";

        #endregion

        #region Корзина

        public const string ClearUserBasketById = "clear-basket/{basketId}";

        public const string GetUserBasketOrdersByBasketId = "get-basket-user-orders/{basketId}";

        public const string GetBasketById = "get-basket/{basketId}";

        #endregion
    }
}
