using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.ComplexTypes;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;
using System.Data;

namespace OrderPaymentSystem.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;
        private readonly ICacheService _cacheService;

        public PaymentService(IMapper mapper, IMessageProducer messageProducer,
            IOptions<RabbitMqSettings> rabbitMqOptions, IUnitOfWork unitOfWord, ICacheService cacheService)
        {
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _unitOfWork = unitOfWord;
            _cacheService = cacheService;
        }

        /// <inheritdoc/>
        public async Task<BaseResult<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var basket = await _unitOfWork.Baskets.GetAll().FirstOrDefaultAsync(x => x.Id == dto.BasketId);

            if (basket == null)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorCode = (int)ErrorCodes.BasketNotFound,
                    ErrorMessage = ErrorMessage.BasketNotFound
                };
            }

            var basketOrders = await _unitOfWork.Orders.GetAll()
                .Where(x => x.Id == dto.BasketId)
                .ToListAsync();

            if (basketOrders.Count == 0)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = ErrorMessage.OrdersNotFound,
                    ErrorCode = (int)ErrorCodes.OrdersNotFound
                };
            }

            var costOfBasketOrders = basketOrders.Sum(o => o.OrderCost);

            if (costOfBasketOrders > dto.AmountOfPayment)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = ErrorMessage.NotEnoughPayFunds,
                    ErrorCode = (int)ErrorCodes.NotEnoughPayFunds
                };
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    Payment payment = new Payment()
                    {
                        BasketId = basket.Id,
                        AmountOfPayment = dto.AmountOfPayment,
                        PaymentMethod = dto.PaymentMethod,
                        CostOfOrders = costOfBasketOrders,
                        CashChange = dto.AmountOfPayment - costOfBasketOrders,
                        DeliveryAddress = new Address()
                        {
                            Street = dto.Street,
                            City = dto.City,
                            Country = dto.Country,
                            ZipCode = dto.Zipcode
                        }
                    };

                    await _unitOfWork.Payments.CreateAsync(payment);

                    await _unitOfWork.SaveChangesAsync();

                    basketOrders.ForEach(x => x.BasketId = null);
                    basketOrders.ForEach(x => x.PaymentId = payment.Id);

                    _unitOfWork.Orders.UpdateRange(basketOrders);

                    _messageProducer.SendMessage(payment, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

                    await _unitOfWork.SaveChangesAsync();


                    await transaction.CommitAsync();

                    return new BaseResult<PaymentDto>()
                    {
                        Data = _mapper.Map<PaymentDto>(payment),
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<PaymentDto>()
            {
                ErrorCode = (int)ErrorCodes.InternalServerError,
                ErrorMessage = ErrorMessage.InternalServerError
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<PaymentDto>> DeletePaymentAsync(long id)
        {
            var payment = await _unitOfWork.Payments.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (payment == null)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorCode = (int)ErrorCodes.PaymentNotFound,
                    ErrorMessage = ErrorMessage.PaymentNotFound
                };
            }

            _unitOfWork.Payments.Remove(payment);
            await _unitOfWork.Payments.SaveChangesAsync();

            _messageProducer.SendMessage(payment, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<PaymentDto>()
            {
                Data = _mapper.Map<PaymentDto>(payment),
            };
        }

        public async Task<BaseResult<PaymentDto>> GetPaymentByIdAsync(long id)
        {
            var payment = await _cacheService.GetObjectAsync(
                string.Format(CacheKeys.Payment, id),
                async () =>
                {
                    return await _unitOfWork.Payments.GetAll()
                        .Where(x => x.Id == id)
                        .Select(x => _mapper.Map<PaymentDto>(x))
                        .FirstOrDefaultAsync();
                });

            if (payment == null)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = ErrorMessage.PaymentNotFound,
                    ErrorCode = (int)ErrorCodes.PaymentNotFound
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
            var payment = await _unitOfWork.Payments.GetAll()
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (payment == null)
            {
                return new CollectionResult<OrderDto>()
                {
                    ErrorCode = (int)ErrorCodes.PaymentNotFound,
                    ErrorMessage = ErrorMessage.PaymentNotFound
                };
            }

            var result = payment.Orders
                .Where(x => x.PaymentId == payment.Id)
                .Select(x => _mapper.Map<OrderDto>(x)).ToList();

            return new CollectionResult<OrderDto>()
            {
                Count = result.Count,
                Data = result
            };
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<PaymentDto>> GetUserPaymentsAsync(long userId)
        {
            var userPayments = await _cacheService.GetObjectAsync(
                string.Format(CacheKeys.UserPayments, userId),
                async () =>
                {
                    return await _unitOfWork.Payments.GetAll()
                        .Include(x => x.Basket)
                        .Where(x => x.Basket.UserId == userId)
                        .Select(x => _mapper.Map<PaymentDto>(x))
                        .ToArrayAsync();
                });


            if (userPayments.Length == 0)
            {
                return new CollectionResult<PaymentDto>()
                {
                    ErrorMessage = ErrorMessage.PaymentsNotFound,
                    ErrorCode = (int)ErrorCodes.PaymentsNotFound
                };
            }

            _messageProducer.SendMessage(userPayments, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new CollectionResult<PaymentDto>()
            {
                Data = userPayments,
                Count = userPayments.Length
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto)
        {
            var payment = await _unitOfWork.Payments.GetAll()
                .Include(x => x.Basket.Orders)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (payment == null)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorCode = (int)ErrorCodes.PaymentNotFound,
                    ErrorMessage = ErrorMessage.PaymentNotFound
                };
            }

            if (payment.AmountOfPayment != dto.AmountOfPayment && payment.PaymentMethod != dto.PaymentMethod)
            {
                var costOfBasketOrders = payment.Basket.Orders.Sum(o => o.OrderCost);

                if (costOfBasketOrders > dto.AmountOfPayment)
                {
                    return new BaseResult<PaymentDto>()
                    {
                        ErrorMessage = ErrorMessage.NotEnoughPayFunds,
                        ErrorCode = (int)ErrorCodes.NotEnoughPayFunds
                    };
                }

                payment.PaymentMethod = dto.PaymentMethod;
                payment.AmountOfPayment = dto.AmountOfPayment;
                payment.CashChange = dto.AmountOfPayment - costOfBasketOrders;

                var updatedPayment = _unitOfWork.Payments.Update(payment);
                await _unitOfWork.Payments.SaveChangesAsync();

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
