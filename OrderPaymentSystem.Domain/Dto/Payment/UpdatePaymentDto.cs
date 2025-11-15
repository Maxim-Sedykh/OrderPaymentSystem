using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Domain.Dto.Payment;

public record UpdatePaymentDto(
        long Id,
        decimal AmountOfPayment,
        PaymentMethod PaymentMethod
    );
