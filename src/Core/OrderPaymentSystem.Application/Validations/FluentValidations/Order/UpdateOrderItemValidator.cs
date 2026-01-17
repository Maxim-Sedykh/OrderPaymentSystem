using FluentValidation;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Order
{
    public class UpdateOrderItemValidator : AbstractValidator<UpdateOrderItemDto>
    {
        public UpdateOrderItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(ErrorMessage.InvalidProductId);

            RuleFor(x => x.NewQuantity)
                .GreaterThan(0)
                .WithMessage(ErrorMessage.QuantityPositive);
        }
    }
}
