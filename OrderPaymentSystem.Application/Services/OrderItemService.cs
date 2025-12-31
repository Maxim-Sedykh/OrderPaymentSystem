using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto;
using OrderPaymentSystem.Domain.Dto.OrderItem;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с элементами заказов
/// </summary>
public class OrderItemService : IOrderItemService
{
    private readonly IBaseRepository<Product> _productRepository;
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IBaseRepository<OrderItem> _orderItemRepository;
    private readonly IOrderItemValidator _orderItemValidator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public OrderItemService(IBaseRepository<Product> productRepository,
        IBaseRepository<Order> orderRepository,
        IBaseRepository<OrderItem> orderItemRepository,
        IOrderItemValidator orderItemValidator,
        IMapper mapper,
        ILogger logger)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
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

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return DataResult<OrderItemDto>.Success(_mapper.Map<OrderItemDto>(orderItem));
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(long orderItemId, CancellationToken cancellationToken = default)
    {
        var orderItem = await _orderItemRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == orderItemId, cancellationToken);
        if (orderItem == null)
        {
            return BaseResult.Failure(ErrorCodes.OrderItemNotFound, ErrorMessage.OrderItemNotFound);
        }

        var order = await _orderRepository.GetQueryable()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderItem.OrderId, cancellationToken);

        if (order == null)
        {
            _logger.LogError("Associated order not found for order item ID: {OrderItemId}. This indicates a data inconsistency.", orderItemId);
            return BaseResult.Failure(ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);
        }

        order.RemoveOrderItem(orderItem);

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var orderItems = await _orderItemRepository.GetQueryable()
            .Where(x => x.OrderId == orderId)
            .AsProjected<OrderItem, OrderItemDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<OrderItemDto>.Success(orderItems);
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderItemDto>> UpdateQuantityAsync(long orderItemId, UpdateQuantityDto dto, CancellationToken cancellationToken = default)
    {
        var orderItem = await _orderItemRepository.GetQueryable()
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == orderItemId, cancellationToken);

        if (orderItem == null)
        {
            return DataResult<OrderItemDto>.Failure(ErrorCodes.OrderItemNotFound, ErrorMessage.OrderItemNotFound);
        }

        var order = await _orderRepository.GetQueryable()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderItem.OrderId, cancellationToken);

        if (order == null)
        {
            _logger.LogError("Associated order not found for order item ID: {OrderItemId}. This indicates a data inconsistency.", orderItemId);
            return DataResult<OrderItemDto>.Failure(ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
        }

        order.UpdateOrderItemQuantity(orderItemId, dto.NewQuantity, orderItem.Product);

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        var updatedOrderItem = order.Items.FirstOrDefault(oi => oi.Id == orderItemId);
        if (updatedOrderItem == null)
        {
            _logger.LogError("Failed to retrieve updated order item after saving for OrderItemId: {OrderItemId}", orderItemId);
            return DataResult<OrderItemDto>.Failure(ErrorCodes.OrderItemNotFound, ErrorMessage.OrderItemNotFound);
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
    private async Task<(Order order, Product product)> GetOrderAndProductAsync(long orderId, long productId, CancellationToken cancellationToken)
    {
        var orderTask = _orderRepository.GetQueryable()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        var productTask = _productRepository.GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

        await Task.WhenAll(orderTask, productTask);

        return (orderTask.Result, productTask.Result);
    }
}
