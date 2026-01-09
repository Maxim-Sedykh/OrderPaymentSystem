using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IMapper mapper,
        ILogger<OrderService> logger,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResult> CompleteProcessingAsync(long orderId, long paymentId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAndProductsAsync(orderId, cancellationToken);
            if (order == null)
            {
                return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
            }

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId, cancellationToken);
            if (payment == null)
            {
                return BaseResult.Failure(DomainErrors.Payment.NotFound(paymentId));
            }

            if (payment.OrderId != orderId)
            {
                return BaseResult.Failure(DomainErrors.Payment.OrderNotAssociated());
            }

            foreach (var orderItem in order.Items)
            {
                orderItem.Product.ReduceStockQuantity(orderItem.Quantity);
            }

            order.AssignPayment(paymentId);
            order.ConfirmOrder();

            _unitOfWork.Orders.Update(order);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return BaseResult.Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict during stock reduction for Order: {OrderId}", orderId);

            await transaction.RollbackAsync(cancellationToken);

            return BaseResult.Failure(DomainErrors.General.ConcurrencyConflict());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error when completing the order");

            await transaction.RollbackAsync(cancellationToken);

            return BaseResult.Failure(DomainErrors.General.InternalServerError());
        }
    }

    public async Task<DataResult<OrderDto>> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var orderItems = new List<OrderItem>();
        var productIds = dto.OrderItems.Select(x => x.ProductId);

        var productsDict = await _unitOfWork.Products
            .GetProductsAsDictionaryByIdAsync(productIds, cancellationToken);

        foreach (var itemDto in dto.OrderItems)
        {
            if (!productsDict.TryGetValue(itemDto.ProductId, out var product) || product == null)
            {
                return DataResult<OrderDto>.Failure(DomainErrors.Product.NotFound(itemDto.ProductId));
            }

            orderItems.Add(OrderItem.Create(itemDto.ProductId, itemDto.Quantity, product.Price, product));
        }

        var order = Order.Create(userId, dto.DeliveryAddress, orderItems);

        await _unitOfWork.Orders.CreateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<DataResult<OrderDto>> GetByIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdQuery(orderId)
            .AsProjected<Order, OrderDto>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
        {
            return DataResult<OrderDto>.Failure(DomainErrors.Order.NotFound(orderId));
        }

        return DataResult<OrderDto>.Success(order);
    }

    public async Task<CollectionResult<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdQuery(userId)
            .AsProjected<Order, OrderDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<OrderDto>.Success(orders);
    }

    public async Task<BaseResult> ShipOrderAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAndProductsAndPaymentsAsync(orderId, cancellationToken);

        if (order == null)
        {
            return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
        }

        foreach (var item in order.Items)
        {
            if (item.Product == null)
            {
                return BaseResult.Failure(DomainErrors.Product.NotFound(item.Product.Id));
            }
        }

        if (order.Payment.Status != PaymentStatus.Succeeded)
        {
            return BaseResult.Failure(DomainErrors.Order.CannotBeConfirmedWithoutPayment());
        }

        order.ShipOrder();

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    public async Task<BaseResult> UpdateBulkOrderItemsAsync(long orderId, List<UpdateOrderItemDto> updateDtos, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(orderId, cancellationToken);
        if (order == null)
        {
            return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
        }

        var productIds = updateDtos.Select(a => a.ProductId).Distinct().ToList();

        var productsDict = await _unitOfWork.Products.GetProductsAsDictionaryByIdAsync(productIds, cancellationToken);

        foreach (var update in updateDtos)
        {
            try
            {
                if (!productsDict.TryGetValue(update.ProductId, out var product))
                {
                    return BaseResult.Failure(DomainErrors.Product.NotFound(update.ProductId));
                }
                order.UpdateOrderItem(update.ProductId, update.NewQuantity, product.Price, product);
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, $"Product with id {update.ProductId} ended");

                return BaseResult.Failure(DomainErrors.Product.StockNotAvailable(update.ProductId, update.NewQuantity));
            }
        }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    public async Task<BaseResult> UpdateStatusAsync(long orderId, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
        }

        order.UpdateStatus(dto.NewStatus);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }
}
