using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.ComplexTypes;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using System.Data;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Cервис для работы с платежами
/// </summary>
/// <param name="mapper">Маппер</param>
/// <param name="unitOfWork">Сервис для транзакций</param>
public class PaymentService(IMapper mapper,
    IUnitOfWork unitOfWork,
    IBaseRepository<Basket> basketRepository,
    IBaseRepository<Payment> paymentRepository,
    IBaseRepository<Order> orderRepository) : IPaymentService
{
    /// <inheritdoc/>
    public async Task<DataResult<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var basket = await basketRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.BasketId, cancellationToken);

        if (basket == null)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.BasketNotFound, ErrorMessage.BasketNotFound);
        }

        var basketOrders = await orderRepository.GetQueryable()
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

        using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                Payment payment = new()
                {
                    BasketId = basket.Id,
                    AmountOfPayment = dto.AmountOfPayment,
                    PaymentMethod = dto.PaymentMethod,
                    CostOfOrders = costOfBasketOrders,
                    CashChange = dto.AmountOfPayment - costOfBasketOrders,
                    DeliveryAddress = new Address()
                    {
                        Street = dto.Street,
                        City = dto.City,
                        Country = dto.Country,
                        ZipCode = dto.Zipcode
                    }
                };

                await paymentRepository.CreateAsync(payment, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                foreach (var order in basketOrders)
                {
                    order.BasketId = null;
                    order.PaymentId = payment.Id;
                }

                orderRepository.UpdateRange(basketOrders);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return DataResult<PaymentDto>.Success(mapper.Map<PaymentDto>(payment));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        return DataResult<PaymentDto>.Failure((int)ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);
    }

    /// <inheritdoc/>
    public async Task<DataResult<PaymentDto>> DeletePaymentAsync(long id, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        paymentRepository.Remove(payment);
        await paymentRepository.SaveChangesAsync();

        return DataResult<PaymentDto>.Success(mapper.Map<PaymentDto>(payment));
    }

    public async Task<DataResult<PaymentDto>> GetPaymentByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetQueryable()
                    .Where(x => x.Id == id)
                    .Select(x => mapper.Map<PaymentDto>(x))
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
        var payment = await paymentRepository.GetQueryable()
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment == null)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        var result = payment.Orders
            .Where(x => x.PaymentId == payment.Id)
            .Select(mapper.Map<OrderDto>).ToList();

        return CollectionResult<OrderDto>.Success(result);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<PaymentDto>> GetUserPaymentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userPayments = await paymentRepository.GetQueryable()
                    .Include(x => x.Basket)
                    .Where(x => x.Basket.UserId == userId)
                    .Select(x => mapper.Map<PaymentDto>(x))
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
        var payment = await paymentRepository.GetQueryable()
            .Include(x => x.Basket.Orders)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (payment == null)
        {
            return DataResult<PaymentDto>.Failure((int)ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
        }

        if (payment.AmountOfPayment != dto.AmountOfPayment && payment.PaymentMethod != dto.PaymentMethod)
        {
            var costOfBasketOrders = payment.Basket.Orders.Sum(o => o.OrderCost);

            if (costOfBasketOrders > dto.AmountOfPayment)
            {
                return DataResult<PaymentDto>.Failure((int)ErrorCodes.NotEnoughPayFunds, ErrorMessage.NotEnoughPayFunds);
            }

            payment.PaymentMethod = dto.PaymentMethod;
            payment.AmountOfPayment = dto.AmountOfPayment;
            payment.CashChange = dto.AmountOfPayment - costOfBasketOrders;

            var updatedPayment = paymentRepository.Update(payment);
            await paymentRepository.SaveChangesAsync(cancellationToken);

            return DataResult<PaymentDto>.Success(mapper.Map<PaymentDto>(updatedPayment));
        }

        return DataResult<PaymentDto>.Failure((int)ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
    }
}
