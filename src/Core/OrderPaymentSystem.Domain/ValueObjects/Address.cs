using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Domain.ValueObjects;

public record Address(
    string Street,
    string City,
    string ZipCode,
    string Country)
{
    public static Address TryCreate(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new BusinessException(StreetEmptyCode, StreetEmptyMessage);
        if (string.IsNullOrWhiteSpace(city)) throw new BusinessException(CityEmptyCode, CityEmptyMessage);
        if (string.IsNullOrWhiteSpace(postalCode)) throw new BusinessException(PostalCodeEmptyCode, PostalCodeEmptyMessage);
        if (string.IsNullOrWhiteSpace(country)) throw new BusinessException(CountryEmptyCode, CountryEmptyMessage);

        return new Address(street, city, postalCode, country);
    }
}