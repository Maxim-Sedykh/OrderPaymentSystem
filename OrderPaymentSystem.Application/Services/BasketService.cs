using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;

namespace OrderPaymentSystem.Application.Services;

public class BasketService : IBasketService
{
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IBaseRepository<Basket> _basketRepository;
    private readonly IMapper _mapper;

    public BasketService(IBaseRepository<Order> orderRepository, IMapper mapper, IBaseRepository<Basket> basketRepository)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _basketRepository = basketRepository;
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> ClearBasketAsync(long id)
    {
        var basket = await _basketRepository.GetAll()
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (basket == null)
        {
            return new CollectionResult<OrderDto>()
            {
                ErrorCode = (int)ErrorCodes.BasketNotFound,
                ErrorMessage = ErrorMessage.BasketNotFound
            };
        }

        var basketOrders = await _orderRepository.GetAll()
            .Where(x => x.BasketId == basket.Id)
            .ToListAsync();

        if (basketOrders.Count == 0)
        {
            return new CollectionResult<OrderDto>()
            {
                ErrorMessage = ErrorMessage.OrdersNotFound,
                ErrorCode = (int)ErrorCodes.OrdersNotFound
            };
        }

        _orderRepository.RemoveRange(basketOrders);
        await _orderRepository.SaveChangesAsync();

        return new CollectionResult<OrderDto>()
        {
            Data = basketOrders.Select(x => _mapper.Map<OrderDto>(x)),
            Count = basketOrders.Count
        };
    }

    /// <inheritdoc/>
    public async Task<BaseResult<BasketDto>> GetBasketByIdAsync(long basketId)
    {
        var basket = await _basketRepository.GetAll()
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == basketId);

        if (basket == null)
        {
            return new BaseResult<BasketDto>()
            {
                ErrorCode = (int)ErrorCodes.BasketNotFound,
                ErrorMessage = ErrorMessage.BasketNotFound
            };
        }

        return new BaseResult<BasketDto>()
        {
            Data = _mapper.Map<BasketDto>(basket)
        };
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> GetBasketOrdersAsync(long basketId)
    {
        OrderDto[] userBasketOrders;

        userBasketOrders = await _orderRepository.GetAll()
            .Where(x => x.BasketId == basketId)
            .Select(x => _mapper.Map<OrderDto>(x))
            .ToArrayAsync();

        if (userBasketOrders.Length == 0)
        {
            return new CollectionResult<OrderDto>()
            {
                ErrorMessage = ErrorMessage.OrdersNotFound,
                ErrorCode = (int)ErrorCodes.OrdersNotFound
            };
        }

        return new CollectionResult<OrderDto>()
        {
            Data = userBasketOrders,
            Count = userBasketOrders.Length
        };
    }
}
