using MapsterMapper;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

internal class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BaseResult> CompletePaymentAsync(long paymentId, CompletePaymentDto dto, CancellationToken ct = default)
    {
        var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(PaymentSpecs.ById(paymentId), ct);
        if (payment == null)
        {
            return BaseResult.Failure(DomainErrors.Payment.NotFound(paymentId));
        }

        payment.ProcessPayment(dto.AmountPaid, dto.CashChange);

        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    public async Task<DataResult<PaymentDto>> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var paymentExists = await _unitOfWork.Payments.AnyAsync(PaymentSpecs.ByOrderId(dto.OrderId), ct);
        if (paymentExists)
        { 
            return DataResult<PaymentDto>.Failure(DomainErrors.Payment.AlreadyExists(dto.OrderId)); 
        }
        
        var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(OrderSpecs.ById(dto.OrderId), ct);
        if (order == null)
        { 
            return DataResult<PaymentDto>.Failure(DomainErrors.Order.NotFound(dto.OrderId));
        }

        var payment = Payment.Create(dto.OrderId, dto.AmountPayed, order.TotalAmount, dto.Method);

        await _unitOfWork.Payments.CreateAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
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
}
