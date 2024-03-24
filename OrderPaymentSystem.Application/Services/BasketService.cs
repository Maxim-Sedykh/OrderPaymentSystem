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
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;

namespace OrderPaymentSystem.Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<Basket> _basketRepository;
        private readonly IBaseValidator<Basket> _basketValidator;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;

        public BasketService(IBaseRepository<Order> orderRepository, IMapper mapper, IMessageProducer messageProducer,
            IOptions<RabbitMqSettings> rabbitMqOptions, IBaseRepository<Basket> basketRepository, IBaseValidator<Basket> basketValidator)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _basketRepository = basketRepository;
            _basketValidator = basketValidator;
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<OrderDto>> ClearBasketAsync(long id)
        {
            var basket = await _basketRepository.GetAll()
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == id);

            var basketNullValidationResult = _basketValidator.ValidateOnNull(basket);
            if (!basketNullValidationResult.IsSuccess)
            {
                return new CollectionResult<OrderDto>()
                {
                    ErrorMessage = basketNullValidationResult.ErrorMessage,
                    ErrorCode = basketNullValidationResult.ErrorCode
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

            var basketNullValidationResult = _basketValidator.ValidateOnNull(basket);
            if (!basketNullValidationResult.IsSuccess)
            {
                return new BaseResult<BasketDto>()
                {
                    ErrorMessage = basketNullValidationResult.ErrorMessage,
                    ErrorCode = basketNullValidationResult.ErrorCode
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
                .Select(x =>  _mapper.Map<OrderDto>(x))
                .ToArrayAsync();

            if (userBasketOrders.Length == 0)
            {
                return new CollectionResult<OrderDto>()
                {
                    ErrorMessage = ErrorMessage.OrdersNotFound,
                    ErrorCode = (int)ErrorCodes.OrdersNotFound
                };
            }

            _messageProducer.SendMessage(userBasketOrders, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new CollectionResult<OrderDto>()
            {
                Data = userBasketOrders,
                Count = userBasketOrders.Length
            };
        }
    }
}
