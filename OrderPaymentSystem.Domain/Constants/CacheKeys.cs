namespace OrderPaymentSystem.Domain.Constants;

/// <summary>
/// Класс констант для хранения значений ключей для кэширования в Redis
/// </summary>
public static class CacheKeys
{
    public const string Product = "product:productId:{0}";
    public const string Products = "products";
    public const string Order = "order:orderId:{0}";
    public const string Payment = "payment:{0}";
    public const string UserPayments = "userPayments:userId:{0}";
    public const string Roles = "roles";
}
