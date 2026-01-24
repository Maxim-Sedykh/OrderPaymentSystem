using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResult> CompletePaymentAsync(long paymentId, CompletePaymentDto dto, CancellationToken ct = default)
    {
        var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(PaymentSpecs.ById(paymentId), ct);
        if (payment == null)
        {
            return BaseResult.Failure(DomainErrors.Payment.NotFound(paymentId));
        }

        payment.ProcessPayment(dto.AmountPaid, dto.CashChange);

        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    public async Task<BaseResult> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var (orderExists, paymentExists) = await IsExistsPaymentAndOrderAsync(dto.OrderId, ct);
        if (!orderExists) return BaseResult.Failure(DomainErrors.Order.NotFound(dto.OrderId));
        if (paymentExists) return BaseResult.Failure(DomainErrors.Payment.AlreadyExists(dto.OrderId));

        var payment = Payment.Create(dto.OrderId, dto.AmountToPay, dto.Method);

        await _unitOfWork.Payments.CreateAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    public async Task<DataResult<PaymentDto>> GetByIdAsync(long paymentId, CancellationToken ct = default)
    {
        var payment = await _unitOfWork.Payments
            .GetProjectedAsync<PaymentDto>(PaymentSpecs.ById(paymentId), ct);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure(DomainErrors.Payment.NotFound(paymentId));
        }

        return DataResult<PaymentDto>.Success(payment);
    }

    public async Task<CollectionResult<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default)
    {
        var payments = await _unitOfWork.Payments
            .GetListProjectedAsync<PaymentDto>(PaymentSpecs.ByOrderId(orderId), ct);

        return CollectionResult<PaymentDto>.Success(payments);
    }

    private async Task<(bool orderExists, bool paymentExists)> IsExistsPaymentAndOrderAsync(long orderId, CancellationToken ct = default)
    {
        var orderExistsTask = _unitOfWork.Orders.AnyAsync(OrderSpecs.ById(orderId), ct);

        var paymentExistsTask = _unitOfWork.Payments.AnyAsync(PaymentSpecs.ByOrderId(orderId), ct);

        await Task.WhenAll(orderExistsTask, paymentExistsTask);

        return (orderExistsTask.Result, paymentExistsTask.Result);
    }
}
