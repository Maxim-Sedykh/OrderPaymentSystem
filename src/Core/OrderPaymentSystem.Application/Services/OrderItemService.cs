using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.OrderItem;
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

/// <summary>
/// Сервис для работы с элементами заказов
/// </summary>
public class OrderItemService : IOrderItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderItemValidator _orderItemValidator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public OrderItemService(IUnitOfWork unitOfWork,
        IOrderItemValidator orderItemValidator,
        IMapper mapper,
        ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _orderItemValidator = orderItemValidator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderItemDto>> CreateAsync(CreateOrderItemDto dto, CancellationToken cancellationToken = default)
    {
        var (order, product) = await GetOrderAndProductAsync(dto.OrderId, dto.ProductId, cancellationToken);

        var validateUpdatingOrderResult = _orderItemValidator.ValidateUpdatingOrder(order, product);
        if (!validateUpdatingOrderResult.IsSuccess)
        {
            return DataResult<OrderItemDto>.Failure(validateUpdatingOrderResult.Error);
        }

        var orderItem = OrderItem.Create(product.Id, dto.Quantity, product.Price, product);

        order.AddOrderItem(orderItem, product);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<OrderItemDto>.Success(_mapper.Map<OrderItemDto>(orderItem));
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(long orderItemId, CancellationToken cancellationToken = default)
    {
        var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(orderItemId, cancellationToken);
        if (orderItem == null)
        {
            return BaseResult.Failure(DomainErrors.Order.ItemNotFound(orderItemId));
        }

        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(orderItem.OrderId, cancellationToken);

        if (order == null)
        {
            _logger.LogError("Matched order not found for order item with id: {OrderItemId}", orderItemId);

            return BaseResult.Failure(DomainErrors.Order.NotFound(orderItem.OrderId));
        }

        order.RemoveOrderItem(orderItem);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var orderItems = await _unitOfWork.OrderItems.GetByOrderId(orderId)
            .AsProjected<OrderItem, OrderItemDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<OrderItemDto>.Success(orderItems);
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderItemDto>> UpdateQuantityAsync(long orderItemId, UpdateQuantityDto dto, CancellationToken cancellationToken = default)
    {
        var orderItem = await _unitOfWork.OrderItems.GetByIdWithProductAsync(orderItemId, cancellationToken);

        if (orderItem == null)
        {
            return DataResult<OrderItemDto>.Failure(DomainErrors.Order.ItemNotFound(orderItemId));
        }

        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(orderItem.OrderId, cancellationToken);

        if (order == null)
        {
            _logger.LogError("Associated order not found for order item ID: {OrderItemId}. This indicates a data inconsistency.", orderItemId);
            return DataResult<OrderItemDto>.Failure(DomainErrors.Order.NotFound(orderItem.OrderId));
        }

        order.UpdateOrderItemQuantity(orderItemId, dto.NewQuantity, orderItem.Product);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Заказ и товар</returns>
    private async Task<(Order order, Product product)> GetOrderAndProductAsync(long orderId, int productId, CancellationToken cancellationToken)
    {
        var orderTask = _unitOfWork.Orders.GetByIdWithItemsAsync(orderId, cancellationToken);
        var productTask = _unitOfWork.Products.GetByIdAsync(productId, asNoTracking: true, cancellationToken);

        await Task.WhenAll(orderTask, productTask);

        return (orderTask.Result, productTask.Result);
    }
}
