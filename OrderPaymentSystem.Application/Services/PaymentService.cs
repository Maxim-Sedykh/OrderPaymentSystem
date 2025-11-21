using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.ValueObjects;
using System.Data;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Cервис для работы с платежами
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<Basket> _basketRepository;
    private readonly IBaseRepository<Payment> _paymentRepository;
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IBaseRepository<Basket> basketRepository,
        IBaseRepository<Payment> paymentRepository,
        IBaseRepository<Order> orderRepository,
        ILogger<PaymentService> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _basketRepository = basketRepository;
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }


    /// <inheritdoc/>
    public async Task<DataResult<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.BasketId, cancellationToken);

        if (basket == null)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.BasketNotFound, ErrorMessage.BasketNotFound);
        }

        var basketOrders = await _orderRepository.GetQueryable()
            .Where(x => x.Id == dto.BasketId)
            .ToArrayAsync(cancellationToken);

        if (basketOrders.Length == 0)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.OrdersNotFound, ErrorMessage.OrdersNotFound);
        }

        var costOfBasketOrders = basketOrders.Sum(o => o.OrderCost);

        if (costOfBasketOrders > dto.AmountOfPayment)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.NotEnoughPayFunds, ErrorMessage.NotEnoughPayFunds);
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var payment = await CreatePaymentWithOrdersAsync(dto, basket.Id, costOfBasketOrders, basketOrders, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return DataResult<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error while creating payment");

            return DataResult<PaymentDto>.Failure((int)ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);
        }
    }

    /// <inheritdoc/>
    public async Task<DataResult<PaymentDto>> DeletePaymentAsync(long id, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        _paymentRepository.Remove(payment);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return DataResult<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
    }

    public async Task<DataResult<PaymentDto>> GetPaymentByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetQueryable()
            .Where(x => x.Id == id)
            .AsProjected<Payment, PaymentDto>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        return DataResult<PaymentDto>.Success(payment);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> GetPaymentOrdersAsync(long id, CancellationToken cancellationToken = default)
    {
        var paymentExists = await _paymentRepository.GetQueryable()
            .AsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);

        if (!paymentExists)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        var paymentOrders = await _orderRepository.GetQueryable()
            .Where(x => x.PaymentId == id)
            .AsProjected<Order, OrderDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        if (paymentOrders.Length == 0)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.OrdersNotFound, ErrorMessage.OrdersNotFound);
        }

        return CollectionResult<OrderDto>.Success(paymentOrders);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<PaymentDto>> GetUserPaymentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userPayments = await _paymentRepository.GetQueryable()
                    .Include(x => x.Basket)
                    .Where(x => x.Basket.UserId == userId)
                    .AsProjected<Payment, PaymentDto>(_mapper)
                    .ToArrayAsync(cancellationToken);

        if (userPayments.Length == 0)
        {
            return CollectionResult<PaymentDto>.Failure((int)ErrorCodes.PaymentsNotFound, ErrorMessage.PaymentsNotFound);
        }

        return CollectionResult<PaymentDto>.Success(userPayments);
    }

    /// <inheritdoc/>
    public async Task<DataResult<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetQueryable()
            .Include(x => x.Basket.Orders)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        if (!HasPaymentChanges(payment, dto))
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
        }

        var validationResult = await ValidatePaymentUpdateAsync(payment, dto.AmountOfPayment);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        UpdatePaymentProperties(payment, dto);

        var updatedPayment = _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return DataResult<PaymentDto>.Success(_mapper.Map<PaymentDto>(updatedPayment));
    }

    /// <summary>
    /// Создает платеж и привязывает к нему заказы из корзины
    /// </summary>
    /// <param name="dto">DTO создания платежа</param>
    /// <param name="basketId">ID корзины</param>
    /// <param name="totalCost">Общая стоимость заказов</param>
    /// <param name="basketOrders">Заказы в корзине</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданный платеж</returns>
    private async Task<Payment> CreatePaymentWithOrdersAsync(
        CreatePaymentDto dto,
        long basketId,
        decimal totalCost,
        Order[] basketOrders,
        CancellationToken cancellationToken)
    {
        var payment = new Payment
        {
            BasketId = basketId,
            AmountOfPayment = dto.AmountOfPayment,
            PaymentMethod = dto.PaymentMethod,
            CostOfOrders = totalCost,
            CashChange = dto.AmountOfPayment - totalCost,
            DeliveryAddress = new Address
            {
                Street = dto.Street,
                City = dto.City,
                Country = dto.Country,
                ZipCode = dto.Zipcode
            }
        };

        await _paymentRepository.CreateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LinkOrdersToPaymentAsync(basketOrders, payment.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return payment;
    }

    /// <summary>
    /// Привязывает заказы к платежу
    /// </summary>
    /// <param name="orders">Заказы для привязки</param>
    /// <param name="paymentId">ID платежа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    private async Task LinkOrdersToPaymentAsync(Order[] orders, long paymentId, CancellationToken cancellationToken)
    {
        foreach (var order in orders)
        {
            order.BasketId = null;
            order.PaymentId = paymentId;
        }

        _orderRepository.UpdateRange(orders);
    }

    /// <summary>
    /// Проверяет наличие изменений в платеже
    /// </summary>
    /// <param name="payment">Платеж</param>
    /// <param name="dto">DTO обновления</param>
    /// <returns>True если есть изменения, иначе false</returns>
    private static bool HasPaymentChanges(Payment payment, UpdatePaymentDto dto)
    {
        return payment.AmountOfPayment != dto.AmountOfPayment ||
               payment.PaymentMethod != dto.PaymentMethod;
    }

    /// <summary>
    /// Проверяет валидность обновления платежа
    /// </summary>
    /// <param name="payment">Платеж</param>
    /// <param name="newAmount">Новая сумма платежа</param>
    /// <returns>Результат валидации</returns>
    private static async Task<DataResult<PaymentDto>> ValidatePaymentUpdateAsync(Payment payment, decimal newAmount)
    {
        var ordersCost = payment.Orders.Sum(o => o.OrderCost);

        if (ordersCost > newAmount)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.NotEnoughPayFunds, ErrorMessage.NotEnoughPayFunds);
        }

        return DataResult<PaymentDto>.Success(default!);
    }

    /// <summary>
    /// Обновляет свойства платежа
    /// </summary>
    /// <param name="payment">Платеж для обновления</param>
    /// <param name="dto">DTO с новыми значениями</param>
    private static void UpdatePaymentProperties(Payment payment, UpdatePaymentDto dto)
    {
        var ordersCost = payment.Orders.Sum(o => o.OrderCost);

        payment.PaymentMethod = dto.PaymentMethod;
        payment.AmountOfPayment = dto.AmountOfPayment;
        payment.CashChange = dto.AmountOfPayment - ordersCost;
    }
}
