namespace OrderPaymentSystem.Domain.Constants;

/// <summary>
/// Содержит константы, определяющие правила валидации для различных сущностей.
/// </summary>
public static class ValidationConstants
{
    /// <summary>
    /// Константы валидации для сущности "Товар".
    /// </summary>
    public static class Product
    {
        /// <summary>
        /// Максимально допустимая длина названия продукта.
        /// </summary>
        public const int MaxNameLength = 100;

        /// <summary>
        /// Максимально допустимая длина описания продукта.
        /// </summary>
        public const int MaxDescriptionLength = 1000;

        /// <summary>
        /// Минимально допустимая длина названия продукта.
        /// </summary>
        public const int MinNameLength = 3;
    }

    /// <summary>
    /// Константы валидации для сущности "Пользователь".
    /// </summary>
    public static class User
    {
        /// <summary>
        /// Максимально допустимая длина логина пользователя.
        /// </summary>
        public const int MaxLoginLength = 50;

        /// <summary>
        /// Максимально допустимая длина пароля пользователя.
        /// </summary>
        public const int MaxPasswordLength = 50;


        /// <summary>
        /// Минимально допустимая длина логина пользователя.
        /// </summary>
        public const int MinLoginLength = 5;

        /// <summary>
        /// Минимально допустимая длина пароля пользователя.
        /// </summary>
        public const int MinPasswordLength = 5;
    }

    /// <summary>
    /// Константы валидации для сущности "Роль".
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// Максимально допустимая длина имени роли.
        /// </summary>
        public const int MaxNameLength = 50;
    }

    /// <summary>
    /// Константы валидации для сущности "Адрес".
    /// </summary>
    public static class Address
    {
        /// <summary>
        /// Максимально допустимая длина названия улицы.
        /// </summary>
        public const int MaxStreetLength = 50;

        /// <summary>
        /// Максимально допустимая длина названия города.
        /// </summary>
        public const int MaxCityLength = 50;

        /// <summary>
        /// Максимально допустимая длина почтового индекса.
        /// </summary>
        public const int MaxZipCodeLength = 50;
    }
}
