using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с заказами
/// </summary>
/// <param name="orderRepository">Репозиторий для работы с заказами</param>
/// <param name="userRepository">Репозиторий для работы с пользователями</param>
/// <param name="productRepository">Репозиторий для работы с товарами</param>
/// <param name="mapper">Маппер</param>
/// <param name="orderValidator">Валидатор заказов</param>
public class OrderService(IBaseRepository<Order> orderRepository,
    IBaseRepository<User> userRepository,
    IBaseRepository<Product> productRepository,
    IMapper mapper,
    IOrderValidator orderValidator) : IOrderService
{
    /// <inheritdoc/>
    public async Task<DataResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetQueryable()
            .Include(x => x.Basket)
            .FirstOrDefaultAsync(x => x.Id == dto.UserId, cancellationToken);

        var product = await productRepository
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == dto.ProductId, cancellationToken);

        var validateCreatingOrderResult = orderValidator.ValidateCreatingOrder(user, product);
        if (!validateCreatingOrderResult.IsSuccess)
        {
            return DataResult<OrderDto>.Failure(validateCreatingOrderResult.Error);
        }

        Order order = new()
        {
            UserId = user.Id,
            ProductId = dto.ProductId,
            BasketId = user.Basket.Id,
            PaymentId = null,
            ProductCount = dto.ProductCount,
            OrderCost = product.Cost * dto.ProductCount
        };

        await orderRepository.CreateAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return DataResult<OrderDto>.Success(mapper.Map<OrderDto>(order));
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderDto>> DeleteOrderByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            return DataResult<OrderDto>.Failure((int)ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
        }

        orderRepository.Remove(order);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return DataResult<OrderDto>.Success(mapper.Map<OrderDto>(order));
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderDto>> GetOrderByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetQueryable()
                    .Where(x => x.Id == id)
                    .Select(x => mapper.Map<OrderDto>(x))
                    .FirstOrDefaultAsync();

        if (order == null)
        {
            return DataResult<OrderDto>.Failure((int)ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
        }

        return DataResult<OrderDto>.Success(order);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.GetQueryable()
            .Include(x => x.Basket)
            .Select(x => mapper.Map<OrderDto>(x))
            .ToArrayAsync();

        if (orders.Length == 0)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.ProductsNotFound, ErrorMessage.ProductsNotFound);
        }

        return CollectionResult<OrderDto>.Success(orders);
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderDto>> UpdateOrderAsync(UpdateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        var product = await productRepository
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

        var validateUpdatingOrderResult = orderValidator.ValidateUpdatingOrder(order, product);
        if (!validateUpdatingOrderResult.IsSuccess)
        {
            return DataResult<OrderDto>.Failure(validateUpdatingOrderResult.Error);
        }

        if (order.ProductId != dto.ProductId || order.ProductCount != dto.ProductCount)
        {
            order.ProductId = product.Id;
            order.ProductCount = dto.ProductCount;
            order.OrderCost = product.Cost * dto.ProductCount;

            var updatedOrder = orderRepository.Update(order);
            await orderRepository.SaveChangesAsync();

            return DataResult<OrderDto>.Success(mapper.Map<OrderDto>(updatedOrder));
        }

        return DataResult<OrderDto>.Failure((int)ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
    }
}
