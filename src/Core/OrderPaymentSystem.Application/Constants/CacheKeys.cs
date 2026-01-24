namespace OrderPaymentSystem.Application.Constants;

/// <summary>
/// Класс констант для хранения значений ключей для кэширования в Redis
/// </summary>
public static class CacheKeys
{
    public static class Product
    {
        public const string All = "products_all";
        public static string ById(int id) => $"product_{id}";
    }

    public static class Role
    {
        public const string All = "roles_all";
    }

    public static class User
    {
        public static string Roles(Guid userId) => $"user_roles_{userId}";
    }
}