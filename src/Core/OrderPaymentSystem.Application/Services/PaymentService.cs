using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentValidator _paymentValidator;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork,
        IMapper mapper,
        IPaymentValidator paymentValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _paymentValidator = paymentValidator;
    }

    public async Task<BaseResult> CompletePaymentAsync(long paymentId, decimal amountPaid, decimal cashChange, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            return BaseResult.Failure(DomainErrors.Payment.NotFound(paymentId));
        }

        payment.ProcessPayment(amountPaid, cashChange);

        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    public async Task<BaseResult> CreateAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var (orderExists, paymentExists) = await IsExistsPaymentAndOrderAsync(dto.OrderId, cancellationToken);
        var validateCreatingPaymentResult = _paymentValidator.ValidateCreatingPayment(orderExists, paymentExists, dto.OrderId);
        if (!validateCreatingPaymentResult.IsSuccess)
        {
            return BaseResult.Failure(validateCreatingPaymentResult.Error);
        }

        var payment = Payment.Create(dto.OrderId, dto.AmountToPay, dto.Method);

        await _unitOfWork.Payments.CreateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    public async Task<DataResult<PaymentDto>> GetByIdAsync(long paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Payments.GetByIdQuery(paymentId)
            .AsProjected<Payment, PaymentDto>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure(DomainErrors.Payment.NotFound(paymentId));
        }

        return DataResult<PaymentDto>.Success(payment);
    }

    public async Task<CollectionResult<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var payments = await _unitOfWork.Payments.GetByOrderIdQuery(orderId)
            .AsProjected<Payment, PaymentDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<PaymentDto>.Success(payments);
    }

    private async Task<(bool orderExists, bool paymentExists)> IsExistsPaymentAndOrderAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var orderExistsTask = _unitOfWork.Orders.ExistsByIdAsync(orderId, cancellationToken);

        var paymentExistsTask = _unitOfWork.Payments.ExistsByOrderIdAsync(orderId, cancellationToken);

        await Task.WhenAll(orderExistsTask, paymentExistsTask);

        return (orderExistsTask.Result, paymentExistsTask.Result);
    }
}
