using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
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
    public class PaymentService : IPaymentService
    {
        private readonly IBaseRepository<Payment> _paymentRepository;
        private readonly IBaseRepository<Basket> _basketRepository;
        private readonly IBaseValidator<Basket> _basketValidator;
        private readonly IPaymentValidator _paymentValidator;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;

        public PaymentService(IBaseRepository<Payment> paymentRepository, IMapper mapper, IMessageProducer messageProducer,
            IOptions<RabbitMqSettings> rabbitMqOptions, IBaseRepository<Basket> basketRepository, IBaseValidator<Basket> basketValidator,
            IPaymentValidator paymentValidator)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _basketRepository = basketRepository;
            _basketValidator = basketValidator;
            _paymentValidator = paymentValidator;
        }

        /// <inheritdoc/>
        public async Task<BaseResult<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var basket = await _basketRepository.GetAll()
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == dto.BasketId);

            var basketNullValidationResult = _basketValidator.ValidateOnNull(basket);
            if (!basketNullValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = basketNullValidationResult.ErrorMessage,
                    ErrorCode = basketNullValidationResult.ErrorCode
                };
            }

            if (basket.Orders.Count == 0)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = ErrorMessage.OrdersNotFound,
                    ErrorCode = (int)ErrorCodes.OrdersNotFound
                };
            }

            var costOfBasketOrders = basket.Orders.Sum(o => o.OrderCost);

            var paymentValidationResult = _paymentValidator.PaymentAmountVlidator(costOfBasketOrders, dto.AmountOfPayment);
            if (!paymentValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = paymentValidationResult.ErrorMessage,
                    ErrorCode = paymentValidationResult.ErrorCode
                };
            }

            Payment payment = new Payment()
            {
                BasketId = basket.Id,
                AmountOfPayment = dto.AmountOfPayment,
                PaymentMethod = dto.PaymentMethod,
                CostOfOrders = costOfBasketOrders,
                CashChange = dto.AmountOfPayment - costOfBasketOrders
            };

            await _paymentRepository.CreateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            _messageProducer.SendMessage(payment, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<PaymentDto>()
            {
                Data = _mapper.Map<PaymentDto>(payment),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<PaymentDto>> DeletePaymentAsync(long id)
        {
            var payment = await _paymentRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            var paymentNullValidationResult = _paymentValidator.ValidateOnNull(payment);

            if (!paymentNullValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = paymentNullValidationResult.ErrorMessage,
                    ErrorCode = paymentNullValidationResult.ErrorCode
                };
            }

            _paymentRepository.Remove(payment);
            await _paymentRepository.SaveChangesAsync();

            _messageProducer.SendMessage(payment, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<PaymentDto>()
            {
                Data = _mapper.Map<PaymentDto>(payment),
            };
        }

        public async Task<BaseResult<PaymentDto>> GetPaymentByIdAsync(long id)
        {
            var payment = await _paymentRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            var paymentNullValidationResult = _paymentValidator.ValidateOnNull(payment);

            if (!paymentNullValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = paymentNullValidationResult.ErrorMessage,
                    ErrorCode = paymentNullValidationResult.ErrorCode
                };
            }

            return new BaseResult<PaymentDto>()
            {
                Data = _mapper.Map<PaymentDto>(payment),
            };
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<OrderDto>> GetPaymentOrdersAsync(long id)
        {
            var payment = await _paymentRepository.GetAll()
                .Include(x => x.Basket.Orders)
                .FirstOrDefaultAsync(x => x.Id == id);

            var paymentNullValidationResult = _paymentValidator.ValidateOnNull(payment);

            if (!paymentNullValidationResult.IsSuccess)
            {
                return new CollectionResult<OrderDto>()
                {
                    ErrorMessage = paymentNullValidationResult.ErrorMessage,
                    ErrorCode = paymentNullValidationResult.ErrorCode
                };
            }

            var result = payment.Basket.Orders.Select(x => _mapper.Map<OrderDto>(x)).ToList();

            return new CollectionResult<OrderDto>()
            {
                Count = result.Count,
                Data = result
            };
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<PaymentDto>> GetUserPaymentsAsync(long userId)
        {
            PaymentDto[] payments;

            payments = await _paymentRepository.GetAll()
                .Select(x => _mapper.Map<PaymentDto>(x))
                .ToArrayAsync();

            if (payments.Length == 0)
            {
                return new CollectionResult<PaymentDto>()
                {
                    ErrorMessage = ErrorMessage.PaymentsNotFound,
                    ErrorCode = (int)ErrorCodes.PaymentsNotFound
                };
            }

            _messageProducer.SendMessage(payments, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new CollectionResult<PaymentDto>()
            {
                Data = payments,
                Count = payments.Length
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto)
        {
            var payment = await _paymentRepository.GetAll()
                .Include(x => x.Basket.Orders)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);
            var paymentNullValidationResult = _paymentValidator.ValidateOnNull(payment);

            if (!paymentNullValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = paymentNullValidationResult.ErrorMessage,
                    ErrorCode = paymentNullValidationResult.ErrorCode
                };
            }

            if (payment.AmountOfPayment != dto.AmountOfPayment && payment.PaymentMethod != dto.PaymentMethod)
            {
                var costOfBasketOrders = payment.Basket.Orders.Sum(o => o.OrderCost);

                var paymentCreateValidationResult = _paymentValidator.PaymentAmountVlidator(costOfBasketOrders, dto.AmountOfPayment);
                if (!paymentCreateValidationResult.IsSuccess)
                {
                    return new BaseResult<PaymentDto>()
                    {
                        ErrorMessage = paymentCreateValidationResult.ErrorMessage,
                        ErrorCode = paymentCreateValidationResult.ErrorCode
                    };
                }

                payment.PaymentMethod = dto.PaymentMethod;
                payment.AmountOfPayment = dto.AmountOfPayment;
                payment.CashChange = dto.AmountOfPayment - costOfBasketOrders;

                var updatedPayment = _paymentRepository.Update(payment);
                await _paymentRepository.SaveChangesAsync();

                _messageProducer.SendMessage(updatedPayment, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

                return new BaseResult<PaymentDto>()
                {
                    Data = _mapper.Map<PaymentDto>(updatedPayment)
                };
            }

            return new BaseResult<PaymentDto>()
            {
                ErrorMessage = ErrorMessage.NoChangesFound,
                ErrorCode = (int)ErrorCodes.NoChangesFound
            };
        }
    }
}
