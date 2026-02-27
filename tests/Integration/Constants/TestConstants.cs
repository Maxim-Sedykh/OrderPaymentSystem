namespace OrderPaymentSystem.IntegrationTests.Constants;

/// <summary>
/// Класс для хранения всех констант необходимых для тестов
/// </summary>
public static class TestConstants
{
    /// <summary>
    /// Роут для товаров
    /// </summary>
    public const string ApiProductsV1 = "/api/v1/products";

    /// <summary>
    /// Роут для регистрации
    /// </summary>
    public const string ApiAuthRegister = "/api/v1/auth/register";

    /// <summary>
    /// Роут для логина
    /// </summary>
    public const string ApiAuthLogin = "/api/v1/auth/login";

    /// <summary>
    /// Роут для элементов корзины
    /// </summary>
    public const string ApiBasketV1 = "/api/v1/basket";

    /// <summary>
    /// Роут для заказов
    /// </summary>
    public const string ApiOrdersV1 = "/api/v1/orders";

    /// <summary>
    /// Роут для платежей
    /// </summary>
    public const string ApiPaymentsV1 = "/api/v1/payments";

    /// <summary>
    /// Логин админа
    /// </summary>
    public const string AdminLogin = "admin";

    /// <summary>
    /// Пароль админа
    /// </summary>
    public const string AdminPassword = "12345";

    /// <summary>
    /// Моковый логин обычного пользователя
    /// </summary>
    public const string CommonUserLogin = "common_user";

    /// <summary>
    /// Моковый пароль обычного пользователя
    /// </summary>
    public const string CommonUserPassword = "StrongPass123!";

    /// <summary>
    /// Моковый логин второго обычного пользователя 
    /// </summary>
    public const string UserTwoLogin = "user_two";

    /// <summary>
    /// Моковый пароль второго обычного пользователя
    /// </summary>
    public const string UserTwoPassword = "password_for_user_two";

    /// <summary>
    /// Моковое имя для товара
    /// </summary>
    public const string DefaultProductName = "Test Product";

    /// <summary>
    /// Моковое описание товара
    /// </summary>
    public const string DefaultProductDescription = "A product for testing";

    /// <summary>
    /// Моковая цена товара
    /// </summary>
    public const decimal DefaultProductPrice = 100m;

    /// <summary>
    /// Моковое количество товара на складе
    /// </summary>
    public const int DefaultProductStock = 10;
}
