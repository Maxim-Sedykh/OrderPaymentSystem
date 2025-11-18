using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Cервис для работы с корзиной пользователя
/// </summary>
/// <param name="orderRepository">Репозиторий заказов</param>
/// <param name="mapper">Маппер</param>
/// <param name="basketRepository">Репозиторий корзин</param>
public class BasketService(IBaseRepository<Order> orderRepository, IMapper mapper, IBaseRepository<Basket> basketRepository) : IBasketService
{
    private readonly IBaseRepository<Order> _orderRepository = orderRepository;
    private readonly IBaseRepository<Basket> _basketRepository = basketRepository;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> ClearBasketAsync(long id, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetQueryable()
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (basket == null)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.BasketNotFound, ErrorMessage.BasketNotFound);
        }

        var basketOrders = await _orderRepository.GetQueryable()
            .Where(x => x.BasketId == basket.Id)
            .ToListAsync(cancellationToken);

        if (basketOrders.Count == 0)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.OrdersNotFound, ErrorMessage.OrdersNotFound);
        }

        _orderRepository.RemoveRange(basketOrders);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return CollectionResult<OrderDto>.Success(basketOrders.Select(_mapper.Map<OrderDto>));
    }

    /// <inheritdoc/>
    public async Task<DataResult<BasketDto>> GetBasketByIdAsync(long basketId, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetQueryable()
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == basketId, cancellationToken);

        if (basket == null)
        {
            return DataResult<BasketDto>.Failure((int)ErrorCodes.BasketNotFound, ErrorMessage.BasketNotFound);
        }

        return DataResult<BasketDto>.Success(_mapper.Map<BasketDto>(basket));
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> GetBasketOrdersAsync(long basketId, CancellationToken cancellationToken = default)
    {
        OrderDto[] userBasketOrders;

        userBasketOrders = await _orderRepository.GetQueryable()
            .Where(x => x.BasketId == basketId)
            .Select(x => _mapper.Map<OrderDto>(x))
            .ToArrayAsync(cancellationToken);

        if (userBasketOrders.Length == 0)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.OrdersNotFound, ErrorMessage.OrdersNotFound);
        }

        return CollectionResult<OrderDto>.Success(userBasketOrders);
    }
}
