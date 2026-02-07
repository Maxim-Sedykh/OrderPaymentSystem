namespace OrderPaymentSystem.Domain.Constants;

public static class ValidationConstants
{
    public static class Product
    {
        public const int MaxNameLength = 100;
        public const int MaxDescriptionLength = 1000;
        public const int MinNameLength = 3;
    }

    public static class User
    {
        public const int MaxLoginLength = 50;
        public const int MaxPasswordLength = 50;

        public const int MinLoginLength = 5;
        public const int MinPasswordLength = 5;
    }

    public static class Role
    {
        public const int MaxNameLength = 50;
    }

    public static class Address
    {
        public const int MaxStreetLength = 50;
        public const int MaxCityLength = 50;
        public const int MaxZipCodeLength = 50;
    }
}
