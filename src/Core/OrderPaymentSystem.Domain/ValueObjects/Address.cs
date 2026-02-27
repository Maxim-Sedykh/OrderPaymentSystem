using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.ValueObjects;

/// <summary>
/// Представляет объект значения "Адрес" с улицей, городом, почтовым индексом и страной.
/// </summary>
/// <param name="Street">Улица.</param>
/// <param name="City">Город.</param>
/// <param name="ZipCode">Почтовый индекс.</param>
/// <param name="Country">Страна.</param>
public record Address(
    string Street,
    string City,
    string ZipCode,
    string Country);