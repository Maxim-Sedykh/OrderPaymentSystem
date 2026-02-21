namespace OrderPaymentSystem.IntegrationTests.Constants;

public static class TestConstants
{
    public const string ApiProductsV1 = "/api/v1/products";
    public const string ApiAuthRegister = "/api/v1/auth/register";
    public const string ApiAuthLogin = "/api/v1/auth/login";
    public const string ApiBasketV1 = "/api/v1/basket";
    public const string ApiOrdersV1 = "/api/v1/orders";
    public const string ApiPaymentsV1 = "/api/v1/payments";

    public const string AdminLogin = "admin";
    public const string AdminPassword = "12345";
    public const string AdminRole = "Admin";

    public const string CommonUserLogin = "common_user";
    public const string CommonUserPassword = "StrongPass123!";
    public const string UserRole = "User";

    public const string UserTwoLogin = "user_two";
    public const string UserTwoPassword = "password_for_user_two";

    public const string DefaultProductName = "Test Product";
    public const string DefaultProductDescription = "A product for testing";
    public const decimal DefaultProductPrice = 100m;
    public const int DefaultProductStock = 10;
}
