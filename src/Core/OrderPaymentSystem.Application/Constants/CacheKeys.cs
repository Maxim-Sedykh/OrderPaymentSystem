namespace OrderPaymentSystem.Application.Constants;

/// <summary>
/// Класс констант для хранения значений ключей для кэширования в Redis
/// </summary>
public static class CacheKeys
{
    #region Методы для форматирования ключей с параметрами

    public static string Product(int productId) => $"product:{productId}";

    public static string Order(long orderId) => $"order:{orderId}";

    public static string Payment(long paymentId) => $"payment:{paymentId}";

    public static string Payment(Guid paymentId) => $"payment:{paymentId}";

    public static string UserPayments(Guid userId) => $"userPayments:{userId}";

    public static string UserRoles(Guid userId) => $"userRoles:{userId}";

    public static string Basket(long basketId) => $"basket:{basketId}";

    #endregion

    #region Константы для ключей без параметров

    public const string Products = "products";

    public const string Roles = "roles";

    public const string AllUsers = "users:all";

    #endregion
}