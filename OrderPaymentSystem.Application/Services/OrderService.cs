using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;

namespace OrderPaymentSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IUserValidator _userValidator;
        private readonly IBaseValidator<Order> _orderValidator;
        private readonly IProductValidator _productValidator;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;

        public OrderService(IBaseRepository<Order> orderRepository, IBaseRepository<User> userRepository,
            IMapper mapper, IMessageProducer messageProducer, IOptions<RabbitMqSettings> rabbitMqOptions, IUserValidator userValidator,
            IBaseValidator<Order> orderValidator, IProductValidator productValidator, IBaseRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _userRepository = userRepository;
            _userValidator = userValidator;
            _orderValidator = orderValidator;
            _productValidator = productValidator;
            _productRepository = productRepository;
        }


        /// <inheritdoc/>
        public async Task<BaseResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Basket)
                .FirstOrDefaultAsync(x => x.Id == dto.UserId);

            var userNullValidationResult = _userValidator.ValidateOnNull(user);
            if (!userNullValidationResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = userNullValidationResult.ErrorMessage,
                    ErrorCode = userNullValidationResult.ErrorCode
                };
            }

            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            var productNullValidationResult = _productValidator.ValidateOnNull(product);
            if (!userNullValidationResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = userNullValidationResult.ErrorMessage,
                    ErrorCode = userNullValidationResult.ErrorCode
                };
            }

            Order order = new Order()
            {
                UserId = user.Id,
                ProductId = dto.ProductId,
                BasketId = user.Basket.Id,
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
            var order = await _orderRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var orderNullValidationResult = _orderValidator.ValidateOnNull(order);

            if (!orderNullValidationResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = orderNullValidationResult.ErrorMessage,
                    ErrorCode = orderNullValidationResult.ErrorCode
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
            var order = await _orderRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            var orderNullValidationResult = _orderValidator.ValidateOnNull(order);

            if (!orderNullValidationResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = orderNullValidationResult.ErrorMessage,
                    ErrorCode = orderNullValidationResult.ErrorCode
                };
            }

            _messageProducer.SendMessage(order, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<OrderDto>()
            {
                Data = _mapper.Map<OrderDto>(order),
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
            var order = await _orderRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            var orderNullValidationResult = _orderValidator.ValidateOnNull(order);

            if (!orderNullValidationResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = orderNullValidationResult.ErrorMessage,
                    ErrorCode = orderNullValidationResult.ErrorCode
                };
            }

            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            var productNullValidationResult = _productValidator.ValidateOnNull(product);
            if (!productNullValidationResult.IsSuccess)
            {
                return new BaseResult<OrderDto>()
                {
                    ErrorMessage = productNullValidationResult.ErrorMessage,
                    ErrorCode = productNullValidationResult.ErrorCode
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
