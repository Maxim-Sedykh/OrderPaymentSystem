using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

internal class UpdateBulkOrderItemsValidator : AbstractValidator<UpdateBulkOrderItemsDto>
{
    public UpdateBulkOrderItemsValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithError(DomainErrors.Order.ItemsEmpty());

        RuleForEach(x => x.Items).SetValidator(new UpdateOrderItemValidator());
    }
}
