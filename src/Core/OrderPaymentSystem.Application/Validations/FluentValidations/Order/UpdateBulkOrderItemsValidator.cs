using FluentValidation;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Domain.Errors;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order;

/// <summary>
/// Валидатор для <see cref="UpdateBulkOrderItemsDto"/>
/// </summary>
public class UpdateBulkOrderItemsValidator : AbstractValidator<UpdateBulkOrderItemsDto>
{
    /// <summary>
    /// Конструктор валидатора, создание правил для полей DTO <see cref="UpdateBulkOrderItemsDto"/>
    /// </summary>
    public UpdateBulkOrderItemsValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithError(DomainErrors.Order.ItemsEmpty());

        RuleForEach(x => x.Items).SetValidator(new UpdateOrderItemValidator());
    }
}
