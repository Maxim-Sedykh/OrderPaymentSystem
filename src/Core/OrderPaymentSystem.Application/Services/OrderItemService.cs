using MapsterMapper;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с элементами заказов
/// </summary>
public class OrderItemService : IOrderItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public OrderItemService(IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderItemDto>> CreateAsync(CreateOrderItemDto dto, CancellationToken ct = default)
    {
        var (order, product) = await GetOrderAndProductAsync(dto.OrderId, dto.ProductId, ct);
        if (order == null)
            return DataResult<OrderItemDto>.Failure(DomainErrors.Order.NotFound(dto.OrderId));
        if (product == null)
            return DataResult<OrderItemDto>.Failure(DomainErrors.Product.NotFound(dto.ProductId));

        var orderItem = OrderItem.Create(product.Id, dto.Quantity, product.Price, product);

        order.AddOrderItem(orderItem, product);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<OrderItemDto>.Success(_mapper.Map<OrderItemDto>(orderItem));
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(long id, CancellationToken ct = default)
    {
        var orderItem = await _unitOfWork.OrderItems.GetFirstOrDefaultAsync(OrderItemSpecs.ById(id), ct);
        if (orderItem == null)
        {
            return BaseResult.Failure(DomainErrors.Order.ItemNotFound(id));
        }

        var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(OrderSpecs.ById(orderItem.OrderId).WithItems(), ct);

        if (order == null)
        {
            _logger.LogError("Matched order not found for order item with id: {OrderItemId}", id);

            return BaseResult.Failure(DomainErrors.Order.NotFound(orderItem.OrderId));
        }

        order.RemoveOrderItem(orderItem);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default)
    {
        var orderItems = await _unitOfWork.OrderItems
            .GetListProjectedAsync<OrderItemDto>(OrderItemSpecs.ByOrderId(orderId), ct);

        return CollectionResult<OrderItemDto>.Success(orderItems);
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderItemDto>> UpdateQuantityAsync(long orderItemId, UpdateQuantityDto dto, CancellationToken ct = default)
    {
        var orderItem = await _unitOfWork.OrderItems.GetFirstOrDefaultAsync(OrderItemSpecs.ById(orderItemId).WithProduct(), ct);

        if (orderItem == null)
        {
            return DataResult<OrderItemDto>.Failure(DomainErrors.Order.ItemNotFound(orderItemId));
        }

        var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(OrderSpecs.ById(orderItem.OrderId).WithItems(), ct);

        if (order == null)
        {
            _logger.LogError("Associated order not found for order item ID: {OrderItemId}. This indicates a data inconsistency.", orderItemId);
            return DataResult<OrderItemDto>.Failure(DomainErrors.Order.NotFound(orderItem.OrderId));
        }

        order.UpdateOrderItemQuantity(orderItemId, dto.NewQuantity, orderItem.Product);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        var updatedOrderItem = order.Items.FirstOrDefault(oi => oi.Id == orderItemId);
        if (updatedOrderItem == null)
        {
            _logger.LogError("Failed to retrieve updated order item after saving for OrderItemId: {OrderItemId}", orderItemId);
            return DataResult<OrderItemDto>.Failure(DomainErrors.Order.ItemNotFound(orderItemId));
        }

        return DataResult<OrderItemDto>.Success(_mapper.Map<OrderItemDto>(updatedOrderItem));
    }

    /// <summary>
    /// Получить заказ и товар параллельно
    /// </summary>
    /// <param name="orderId">Id заказа</param>
    /// <param name="productId">Id товара</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Заказ и товар</returns>
    private async Task<(Order order, Product product)> GetOrderAndProductAsync(long orderId, int productId, CancellationToken ct)
    {
        var orderTask = _unitOfWork.Orders
            .GetFirstOrDefaultAsync(OrderSpecs.ById(orderId).WithItems(), ct);
        var productTask = _unitOfWork.Products
            .GetFirstOrDefaultAsync(ProductSpecs.ByIdNoTracking(productId), ct);

        await Task.WhenAll(orderTask, productTask);

        return (orderTask.Result, productTask.Result);
    }
}
