using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Interfaces.Validators;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IBaseRepository<Payment> _paymentRepository;
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IPaymentValidator _paymentValidator;
    private readonly IMapper _mapper;

    public PaymentService(IBaseRepository<Payment> paymentRepository, IMapper mapper, IBaseRepository<Order> orderRepository, IPaymentValidator paymentValidator)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
        _orderRepository = orderRepository;
        _paymentValidator = paymentValidator;
    }

    public async Task<BaseResult> CompletePaymentAsync(long paymentId, decimal amountPaid, decimal cashChange, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
        if (payment == null)
        {
            return BaseResult.Failure(ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        payment.ProcessPayment(amountPaid, cashChange);

        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

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

        await _paymentRepository.CreateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    public async Task<DataResult<PaymentDto>> GetByIdAsync(long paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetQueryable()
            .Where(x => x.Id == paymentId)
            .AsProjected<Payment, PaymentDto>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure(ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        return DataResult<PaymentDto>.Success(payment);
    }

    public async Task<CollectionResult<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var payments = await _paymentRepository.GetQueryable()
            .Where(x => x.OrderId == orderId)
            .AsProjected<Payment, PaymentDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<PaymentDto>.Success(payments);
    }

    private async Task<(bool orderExists, bool paymentExists)> IsExistsPaymentAndOrderAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var orderExistsTask = _orderRepository.GetQueryable()
            .AnyAsync(x => x.Id == orderId, cancellationToken);

        var paymentExistsTask = _paymentRepository.GetQueryable()
            .AnyAsync(x => x.OrderId == orderId, cancellationToken);

        await Task.WhenAll(orderExistsTask, paymentExistsTask);

        return (orderExistsTask.Result, paymentExistsTask.Result);
    }
}
