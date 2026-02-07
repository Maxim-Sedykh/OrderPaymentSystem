using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Result;
using Order = OrderPaymentSystem.Domain.Entities.Order;

namespace OrderPaymentSystem.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        ILogger<OrderService> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResult> CompleteProcessingAsync(long orderId, long paymentId, CancellationToken ct = default)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var spec = OrderSpecs.ById(orderId).ForShip();
            var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(spec, ct);
            if (order == null)
            {
                return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
            }

            var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(PaymentSpecs.ById(paymentId), ct);
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

            await _unitOfWork.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return BaseResult.Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict during stock reduction for Order: {OrderId}", orderId);

            await transaction.RollbackAsync(ct);

            return BaseResult.Failure(DomainErrors.General.ConcurrencyConflict());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error when completing the order");

            await transaction.RollbackAsync(ct);

            return BaseResult.Failure(DomainErrors.General.InternalServerError());
        }
    }

    public async Task<DataResult<OrderDto>> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken ct = default)
    {
        var orderItems = new List<OrderItem>();
        var productIds = dto.OrderItems.Select(x => x.ProductId);

        var productsDict = await _unitOfWork.Products
            .GetProductsAsDictionaryByIdAsync(productIds, ct);

        foreach (var itemDto in dto.OrderItems)
        {
            if (!productsDict.TryGetValue(itemDto.ProductId, out var product) || product == null)
            {
                return DataResult<OrderDto>.Failure(DomainErrors.Product.NotFound(itemDto.ProductId));
            }

            orderItems.Add(OrderItem.Create(itemDto.ProductId, itemDto.Quantity, product.Price, product));
        }

        var order = Order.Create(userId, dto.DeliveryAddress, orderItems);

        await _unitOfWork.Orders.CreateAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<DataResult<OrderDto>> GetByIdAsync(long orderId, CancellationToken ct = default)
    {
        var order = await _unitOfWork.Orders
            .GetProjectedAsync<OrderDto>(OrderSpecs.ById(orderId), ct);

        if (order == null)
        {
            return DataResult<OrderDto>.Failure(DomainErrors.Order.NotFound(orderId));
        }

        return DataResult<OrderDto>.Success(order);
    }

    public async Task<CollectionResult<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var orders = await _unitOfWork.Orders
            .GetListProjectedAsync<OrderDto>(OrderSpecs.ByUserIdNoTracking(userId), ct);

        return CollectionResult<OrderDto>.Success(orders);
    }

    public async Task<BaseResult> ShipOrderAsync(long orderId, CancellationToken ct = default)
    {
        var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(OrderSpecs.ById(orderId).ForShip(), ct);

        if (order == null)
        {
            return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
        }

        if (order.Payment == null)
        {
            return BaseResult.Failure(DomainErrors.Order.CannotBeConfirmedWithoutPayment());
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

        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    public async Task<BaseResult> UpdateBulkOrderItemsAsync(long orderId, UpdateBulkOrderItemsDto dto, CancellationToken ct = default)
    {
        var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(OrderSpecs.ById(orderId).WithItems(),
            ct);
        if (order == null)
        {
            return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
        }

        var productIds = dto.Items.Select(a => a.ProductId).Distinct().ToList();

        var productsDict = await _unitOfWork.Products.GetProductsAsDictionaryByIdAsync(productIds, ct);

        foreach (var newItem in dto.Items)
        {
            try
            {
                if (!productsDict.TryGetValue(newItem.ProductId, out var product))
                {
                    return BaseResult.Failure(DomainErrors.Product.NotFound(newItem.ProductId));
                }
                order.UpdateOrderItem(newItem.ProductId, newItem.NewQuantity, product.Price, product);
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, $"Product with id {newItem.ProductId} ended");

                return BaseResult.Failure(DomainErrors.Product.StockNotAvailable(newItem.ProductId, newItem.NewQuantity));
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    public async Task<BaseResult> UpdateStatusAsync(long orderId, UpdateOrderStatusDto dto, CancellationToken ct = default)
    {
        var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(OrderSpecs.ById(orderId), ct);
        if (order == null)
        {
            return BaseResult.Failure(DomainErrors.Order.NotFound(orderId));
        }

        order.UpdateStatus(dto.NewStatus);

        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }
}
