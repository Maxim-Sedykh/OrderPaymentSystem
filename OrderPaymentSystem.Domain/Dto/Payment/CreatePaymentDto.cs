using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Domain.Dto.Payment;

public record CreatePaymentDto(
        long BasketId,
        decimal AmountOfPayment,
        PaymentMethod PaymentMethod,
        string Street,
        string City,
        string Country,
        string Zipcode
    );
