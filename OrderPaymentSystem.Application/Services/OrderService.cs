using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис для работы с заказами
/// </summary>
public class OrderService : IOrderService
{
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly IOrderValidator _orderValidator;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Конструктор сервиса для работы с заказами
    /// </summary>
    /// <param name="orderRepository">Репозиторий для работы с заказами</param>
    /// <param name="userRepository">Репозиторий для работы с пользователями</param>
    /// <param name="productRepository">Репозиторий для работы с товарами</param>
    /// <param name="mapper">Маппер</param>
    /// <param name="orderValidator">Валидатор заказов</param>
    /// <param name="cacheService">Сервис для кэширования</param>
    public OrderService(
        IBaseRepository<Order> orderRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<Product> productRepository,
        IMapper mapper,
        IOrderValidator orderValidator,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _orderValidator = orderValidator;
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<BaseResult> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var (user, product) = await GetUserAndProductAsync(dto.UserId, dto.ProductId, cancellationToken);

        var validateCreatingOrderResult = _orderValidator.ValidateCreatingOrder(user, product);
        if (!validateCreatingOrderResult.IsSuccess)
        {
            return BaseResult.Failure(validateCreatingOrderResult.Error);
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

        await _orderRepository.CreateAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteOrderByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (order == null)
        {
            return BaseResult.Failure((int)ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
        }

        _orderRepository.Remove(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderDto>> GetOrderByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetQueryable()
                    .Where(x => x.Id == id)
                    .AsProjected<Order, OrderDto>(_mapper)
                    .FirstOrDefaultAsync(cancellationToken); ;

        if (order == null)
        {
            return DataResult<OrderDto>.Failure((int)ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
        }

        return DataResult<OrderDto>.Success(order);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetQueryable()
            .AsProjected<Order, OrderDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        if (orders.Length == 0)
        {
            return CollectionResult<OrderDto>.Failure((int)ErrorCodes.ProductsNotFound, ErrorMessage.ProductsNotFound);
        }

        return CollectionResult<OrderDto>.Success(orders);
    }

    /// <inheritdoc/>
    public async Task<DataResult<OrderDto>> UpdateOrderAsync(long id, UpdateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var (order, product) = await GetOrderAndProductAsync(id, dto.ProductId, cancellationToken);

        var validateUpdatingOrderResult = _orderValidator.ValidateUpdatingOrder(order, product);
        if (!validateUpdatingOrderResult.IsSuccess)
        {
            return DataResult<OrderDto>.Failure(validateUpdatingOrderResult.Error);
        }

        if (!HasOrderChanges(order, dto))
        {
            return DataResult<OrderDto>.Failure((int)ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
        }

        UpdateOrderProperties(order, product, dto.ProductCount);

        var updatedOrder = _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return DataResult<OrderDto>.Success(_mapper.Map<OrderDto>(updatedOrder));
    }

    /// <summary>
    /// Получить ассинхронно пользователя и товар параллельно.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="productId">Id товара</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь и товар</returns>
    private async Task<(User user, Product product)> GetUserAndProductAsync(Guid userId, long productId, CancellationToken cancellationToken)
    {
        var userTask = _userRepository.GetQueryable()
            .AsNoTracking()
            .Include(x => x.Basket)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        var productTask = _productRepository.GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

        await Task.WhenAll(userTask, productTask);

        return (userTask.Result, productTask.Result);
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
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        var productTask = _productRepository.GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

        await Task.WhenAll(orderTask, productTask);

        return (orderTask.Result, productTask.Result);
    }

    /// <summary>
    /// Были ли изменения у заказа
    /// </summary>
    /// <param name="order">Сущность существующего заказа</param>
    /// <param name="dto">DTO с обновлёнными данными заказа</param>
    /// <returns>True если изменения были</returns>
    private static bool HasOrderChanges(Order order, UpdateOrderDto dto)
    {
        return order.ProductId != dto.ProductId || order.ProductCount != dto.ProductCount;
    }

    /// <summary>
    /// Обновить свойства заказа
    /// </summary>
    /// <param name="order">Существующий заказ</param>
    /// <param name="product">Товар</param>
    /// <param name="productCount">Количество товара в заказе</param>
    private static void UpdateOrderProperties(Order order, Product product, int productCount)
    {
        order.ProductId = product.Id;
        order.ProductCount = productCount;
        order.OrderCost = product.Cost * productCount;
    }
}
