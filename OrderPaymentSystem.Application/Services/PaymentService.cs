﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.ComplexTypes;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Databases;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;
using System.Data;

namespace OrderPaymentSystem.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseValidator<Basket> _basketValidator;
        private readonly IPaymentValidator _paymentValidator;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;

        public PaymentService(IMapper mapper, IMessageProducer messageProducer,
            IOptions<RabbitMqSettings> rabbitMqOptions, IBaseValidator<Basket> basketValidator,
            IPaymentValidator paymentValidator, IUnitOfWork unitOfWord)
        {
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _basketValidator = basketValidator;
            _paymentValidator = paymentValidator;
            _unitOfWork = unitOfWord;
        }

        /// <inheritdoc/>
        public async Task<BaseResult<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var basket = await _unitOfWork.Baskets.GetAll().FirstOrDefaultAsync(x => x.Id == dto.BasketId);

            var basketNullValidationResult = _basketValidator.ValidateOnNull(basket);
            if (!basketNullValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = basketNullValidationResult.ErrorMessage,
                    ErrorCode = basketNullValidationResult.ErrorCode
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

            var paymentValidationResult = _paymentValidator.PaymentAmountVlidator(costOfBasketOrders, dto.AmountOfPayment);
            if (!paymentValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = paymentValidationResult.ErrorMessage,
                    ErrorCode = paymentValidationResult.ErrorCode
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

            var paymentNullValidationResult = _paymentValidator.ValidateOnNull(payment);

            if (!paymentNullValidationResult.IsSuccess)
            {
                return new BaseResult<PaymentDto>()
                {
                    ErrorMessage = paymentNullValidationResult.ErrorMessage,
                    ErrorCode = paymentNullValidationResult.ErrorCode
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
            var payment = await _unitOfWork.Payments.GetAll().FirstOrDefaultAsync(x => x.Id == id);

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
            var payment = await _unitOfWork.Payments.GetAll()
                .Include(x => x.Orders)
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

            var result = payment.Orders.Select(x => _mapper.Map<OrderDto>(x)).ToList();

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

            payments = await _unitOfWork.Payments.GetAll()
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
            var payment = await _unitOfWork.Payments.GetAll()
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
