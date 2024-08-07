﻿using FluentValidation;
using OrderPaymentSystem.Domain.Dto.Payment;

namespace OrderPaymentSystem.Application.Validations.FluentValidations.Payment
{
    public class UpdatePaymentValidation : AbstractValidator<UpdatePaymentDto>
    {
        public UpdatePaymentValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Идентификатор платежа должен быть указан");
            RuleFor(x => x.AmountOfPayment).GreaterThan(0).WithMessage("Оплата должна быть больше нуля");
        }
    }
}
