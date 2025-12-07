using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Cервис для работы с корзиной пользователя
/// </summary>
public class BasketService : IBasketService
{
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IBaseRepository<Basket> _basketRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Конструктор сервиса для работы с корзиной пользователя
    /// </summary>
    /// <param name="orderRepository">Репозиторий заказов</param>
    /// <param name="mapper">Маппер</param>
    /// <param name="basketRepository">Репозиторий корзин</param>
    /// <param name="cacheService">Сервис для работы с кэшем</param>
    public BasketService(
        IBaseRepository<Order> orderRepository,
        IMapper mapper,
        IBaseRepository<Basket> basketRepository,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _basketRepository = basketRepository;
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> ClearBasketAsync(long basketId, CancellationToken cancellationToken = default)
    {
        if (!await BasketExistsAsync(basketId, cancellationToken))
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.BasketNotFound, ErrorMessage.BasketNotFound);
        }

        var basketOrders = await _orderRepository.GetQueryable()
            .Where(x => x.BasketId == basketId)
            .ToListAsync(cancellationToken);

        if (basketOrders.Count == 0)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.OrdersNotFound, ErrorMessage.OrdersNotFound);
        }

        _orderRepository.RemoveRange(basketOrders);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        var resultDtos = basketOrders.Select(_mapper.Map<OrderDto>).ToArray();
        return CollectionResult<OrderDto>.Success(resultDtos);
    }

    /// <inheritdoc/>
    public async Task<DataResult<BasketDto>> GetBasketByIdAsync(long basketId, CancellationToken cancellationToken = default)
    {
        var basket = await _cacheService.GetOrCreateAsync(CacheKeys.Basket(basketId), async (cancellationToken) =>
        {
            return await _basketRepository.GetQueryable()
                .Include(x => x.Orders)
                .AsProjected<Basket, BasketDto>(_mapper)
                .FirstOrDefaultAsync(x => x.Id == basketId, cancellationToken);
        }, cancellationToken: cancellationToken);

        if (basket == null)
        {
            return DataResult<BasketDto>.Failure((int)ErrorCodes.BasketNotFound, ErrorMessage.BasketNotFound);
        }

        return DataResult<BasketDto>.Success(basket);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> GetBasketOrdersAsync(long basketId, CancellationToken cancellationToken = default)
    {
        var userBasketOrders = _cacheService.GetOrCreateAsync(CacheKeys.Order(basketId)
            , async (cancellationToken) =>
            {
                return await _orderRepository.GetQueryable()
                    .Where(x => x.BasketId == basketId)
                    .AsProjected<Order, OrderDto>(_mapper)
                    .ToArrayAsync(cancellationToken);
            }
            );

        var cacheKey = ;
        var cachedOrders = await _cacheService.GetAsync<OrderDto[]>(cacheKey, cancellationToken);
        if (cachedOrders is not null)
        {
            return CollectionResult<OrderDto>.Success(cachedOrders);
        }

        if (!await BasketExistsAsync(basketId, cancellationToken))
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.BasketNotFound, ErrorMessage.BasketNotFound);
        }

        var userBasketOrders = await _orderRepository.GetQueryable()
            .Where(x => x.BasketId == basketId)
            .AsProjected<Order, OrderDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        if (userBasketOrders.Length == 0)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.OrdersNotFound, ErrorMessage.OrdersNotFound);
        }

        return CollectionResult<OrderDto>.Success(userBasketOrders);
    }

    /// <summary>
    /// Существует ли корзина в БД
    /// </summary>
    /// <param name="basketId">Id корзины</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    private async Task<bool> BasketExistsAsync(long basketId, CancellationToken cancellationToken)
    {
        return await _basketRepository.GetQueryable()
            .AsNoTracking()
            .AnyAsync(x => x.Id == basketId, cancellationToken);
    }
}
