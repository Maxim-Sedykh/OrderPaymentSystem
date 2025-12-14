using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Domain.Dto.Payment;

public record UpdatePaymentDto(
        decimal AmountOfPayment,
        PaymentMethod PaymentMethod
    );
