using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Domain.Resources;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

internal class UpdateBulkOrderItemsValidator : AbstractValidator<UpdateBulkOrderItemsDto>
{
    public UpdateBulkOrderItemsValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(ErrorMessage.OrderItemsEmpty);

        RuleForEach(x => x.Items).SetValidator(new UpdateOrderItemValidator());
    }
}
