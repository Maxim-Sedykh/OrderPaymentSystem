using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Domain.ValueObjects;

public record Address(
    string Street,
    string City,
    string ZipCode,
    string Country)
{
    public static Address TryCreate(string street, string city, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new BusinessException(DomainErrors.Address.StreetEmpty());
        if (string.IsNullOrWhiteSpace(city)) throw new BusinessException(DomainErrors.Address.CityEmpty());
        if (string.IsNullOrWhiteSpace(zipCode)) throw new BusinessException(DomainErrors.Address.ZipCodeEmpty());
        if (string.IsNullOrWhiteSpace(country)) throw new BusinessException(DomainErrors.Address.CountryEmpty());

        return new Address(street, city, zipCode, country);
    }
}