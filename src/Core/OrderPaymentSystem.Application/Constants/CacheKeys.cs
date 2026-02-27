namespace OrderPaymentSystem.Application.Constants;

/// <summary>
/// Централизованные ключи кеша, используемые в приложении.
/// Содержит логичные именованные пространства для различных сущностей (Product, Role, User).
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// Ключи кеша, связанные с товарами (Product).
    /// </summary>
    public static class Product
    {
        /// <summary>
        /// Ключ для кеша со списком всех товаров.
        /// Используется при кэшировании полной коллекции продуктов.
        /// </summary>
        public const string All = "products_all";

        /// <summary>
        /// Генерирует ключ для кеша конкретного товара по его идентификатору.
        /// Формат: "product_{id}".
        /// </summary>
        /// <param name="id">Идентификатор товара.</param>
        /// <returns>Строковый ключ кеша для данного товара.</returns>
        public static string ById(int id) => $"product_{id}";
    }

    /// <summary>
    /// Ключи кеша, связанные с ролями (Role).
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// Ключ для кеша со списком всех ролей.
        /// Используется при кэшировании набора ролей (например, для выпадающих списков).
        /// </summary>
        public const string All = "roles_all";
    }

    /// <summary>
    /// Ключи кеша, связанные с пользователями (User).
    /// </summary>
    public static class User
    {
        /// <summary>
        /// Генерирует ключ для кеша ролей конкретного пользователя.
        /// Формат: "user_roles_{userId}".
        /// Используется для кеширования набора ролей, назначенных пользователю.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Строковый ключ кеша для ролей пользователя.</returns>
        public static string Roles(Guid userId) => $"user_roles_{userId}";
    }
}