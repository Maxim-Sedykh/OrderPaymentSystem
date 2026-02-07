using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.ValueObjects;

public record Address(
    string Street,
    string City,
    string ZipCode,
    string Country)
{
    public static Address TryCreate(string street, string city, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new BusinessException(DomainErrors.Validation.Required(nameof(street)));
        if (string.IsNullOrWhiteSpace(city)) throw new BusinessException(DomainErrors.Validation.Required(nameof(city)));
        if (string.IsNullOrWhiteSpace(zipCode)) throw new BusinessException(DomainErrors.Validation.Required(nameof(zipCode)));
        if (string.IsNullOrWhiteSpace(country)) throw new BusinessException(DomainErrors.Validation.Required(nameof(country)));

        return new Address(street, city, zipCode, country);
    }
}