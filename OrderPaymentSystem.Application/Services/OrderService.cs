﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Application.Validations.Validators;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.UserRole;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;

namespace OrderPaymentSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;
        private readonly ICacheService _cacheService;
        private readonly IOrderValidator _orderValidator;

        public OrderService(IBaseRepository<Order> orderRepository, IBaseRepository<User> userRepository, IBaseRepository<Product> productRepository,
            IMapper mapper, IMessageProducer messageProducer, IOptions<RabbitMqSettings> rabbitMqOptions, ICacheService cacheService,
            IOrderValidator orderValidator)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _cacheService = cacheService;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _orderValidator = orderValidator;
        }


        /// <inheritdoc/>
        public async Task<BaseResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Basket)
                .FirstOrDefaultAsync(x => x.Id == dto.UserId);

            var product = await _productRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            var validateCreatingOrderResult = _orderValidator.ValidateCreatingOrder(user, product);
            if (!validateCreatingOrderResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = validateCreatingOrderResult.ErrorMessage,
                    ErrorCode = validateCreatingOrderResult.ErrorCode
                };
            }

            Order order = new Order()
            {
                UserId = user.Id,
                ProductId = dto.ProductId,
                BasketId = user.Basket.Id,
                PaymentId = null,
                ProductCount = dto.ProductCount,
                OrderCost = product.Cost * dto.ProductCount
            };

            await _orderRepository.CreateAsync(order);
            await _orderRepository.SaveChangesAsync();

            _messageProducer.SendMessage(order, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<OrderDto>()
            {
                Data = _mapper.Map<OrderDto>(order),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<OrderDto>> DeleteOrderByIdAsync(long id)
        {
            var order = await _orderRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorCode = (int)ErrorCodes.OrderNotFound,
                    ErrorMessage = ErrorMessage.OrderNotFound
                };
            }

            _orderRepository.Remove(order);
            await _orderRepository.SaveChangesAsync();

            _messageProducer.SendMessage(order, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<OrderDto>()
            {
                Data = _mapper.Map<OrderDto>(order),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<OrderDto>> GetOrderByIdAsync(long id)
        {
            var order = await _cacheService.GetObjectAsync(
                string.Format(CacheKeys.Order, id),
                async () =>
                {
                    return await _orderRepository.GetAll()
                        .Where(x => x.Id == id)
                        .Select(x => _mapper.Map<OrderDto>(x))
                        .FirstOrDefaultAsync();
                });

            if (order == null)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorCode = (int)ErrorCodes.OrderNotFound,
                    ErrorMessage = ErrorMessage.OrderNotFound
                };
            }

            _messageProducer.SendMessage(order, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<OrderDto>()
            {
                Data = order,
            };
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<OrderDto>> GetAllOrdersAsync()
        {
            OrderDto[] orders;

            orders = await _orderRepository.GetAll()
                .Include(x => x.Basket)
                .Select(x => _mapper.Map<OrderDto>(x))
                .ToArrayAsync();

            if (orders.Length == 0)
            {
                return new CollectionResult<OrderDto>()
                {
                    ErrorMessage = ErrorMessage.ProductsNotFound,
                    ErrorCode = (int)ErrorCodes.ProductsNotFound
                };
            }

            _messageProducer.SendMessage(orders, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new CollectionResult<OrderDto>()
            {
                Data = orders,
                Count = orders.Length
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<OrderDto>> UpdateOrderAsync(UpdateOrderDto dto)
        {
            var order = await _orderRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            var product = await _productRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            var validateUpdatingOrderResult = _orderValidator.ValidateUpdatingOrder(order, product);
            if (!validateUpdatingOrderResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = validateUpdatingOrderResult.ErrorMessage,
                    ErrorCode = validateUpdatingOrderResult.ErrorCode
                };
            }

            if (order.ProductId != dto.ProductId || order.ProductCount != dto.ProductCount)
            {
                order.ProductId = product.Id;
                order.ProductCount = dto.ProductCount;
                order.OrderCost = product.Cost * dto.ProductCount;

                var updatedOrder = _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                _messageProducer.SendMessage(order, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

                return new BaseResult<OrderDto>()
                {
                    Data = _mapper.Map<OrderDto>(updatedOrder),
                };
            }

            return new BaseResult<OrderDto>()
            {
                ErrorMessage = ErrorMessage.NoChangesFound,
                ErrorCode = (int)ErrorCodes.NoChangesFound
            };
        }
    }
}
