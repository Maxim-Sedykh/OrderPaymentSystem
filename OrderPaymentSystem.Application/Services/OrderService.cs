using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.OrderItem;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<Payment> _paymentRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IMapper mapper, IBaseRepository<Order> orderRepository, IBaseRepository<Payment> paymentRepository, IBaseRepository<Product> productRepository, ILogger<OrderService> logger)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<BaseResult> CompleteProcessingAsync(long orderId, long paymentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _orderRepository.GetQueryable()
                    .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
                if (order == null)
                {
                    return BaseResult.Failure(ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
                }

                var payment = await _paymentRepository.GetQueryable()
                    .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
                if (payment == null)
                {
                    return BaseResult.Failure(ErrorCodes.PaymentNotFound, ErrorMessage.PaymentNotFound);
                }

                if (payment.OrderId != orderId)
                {
                    return BaseResult.Failure(ErrorCodes.PaymentOrderNotAssociated, ErrorMessage.PaymentOrderNotAssociated);
                }

                foreach (var orderItem in order.Items)
                {
                    orderItem.Product.ReduceStockQuantity(orderItem.Quantity);
                }

                order.AssignPayment(paymentId);
                order.ConfirmOrder();

                _orderRepository.Update(order);

                await _orderRepository.SaveChangesAsync(cancellationToken);

                return BaseResult.Success();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict during stock reduction for Order: {OrderId}", orderId);

                return BaseResult.Failure(ErrorCodes.ConcurrencyConflict, "Товар был изменен другим пользователем. Попробуйте еще раз.");
            }
        }

        public async Task<DataResult<OrderDto>> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default)
        {
            var orderItems = new List<OrderItem>();
            var productIds = dto.OrderItems.Select(x => x.ProductId);

            var products = await _productRepository.GetQueryable()
                    .AsNoTracking()
                    .Where(x => productIds.Contains(x.Id))
                    .ToDictionaryAsync(k => k.Id, v => v);

            foreach (var itemDto in dto.OrderItems)
            {
                if (!products.TryGetValue(itemDto.ProductId, out var product))
                {
                    return DataResult<OrderDto>.Failure(ErrorCodes.ProductNotFound,
                        string.Format(ErrorMessage.ProductNotFound, itemDto.ProductId));
                }

                if (product == null)
                {
                    return DataResult<OrderDto>.Failure(ErrorCodes.ProductNotFound,
                        string.Format(ErrorMessage.ProductNotFound, itemDto.ProductId));
                }

                orderItems.Add(OrderItem.Create(itemDto.ProductId, itemDto.Quantity, product.Price, product));
            }

            var order = Order.Create(userId, dto.DeliveryAddress, orderItems);

            await _orderRepository.CreateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return DataResult<OrderDto>.Success(_mapper.Map<OrderDto>(order));
        }

        public async Task<DataResult<OrderDto>> GetByIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetQueryable()
                .Where(x => x.Id == orderId)
                .AsProjected<Order, OrderDto>(_mapper)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                return DataResult<OrderDto>.Failure(ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
            }

            return DataResult<OrderDto>.Success(order);
        }

        public async Task<CollectionResult<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetQueryable()
                .Where(x => x.UserId == userId)
                .AsProjected<Order, OrderDto>(_mapper)
                .ToArrayAsync(cancellationToken);

            return CollectionResult<OrderDto>.Success(orders);
        }

        public async Task<BaseResult> ShipOrderAsync(long orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.Items)
                .ThenInclude(x => x.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

            if (order == null)
            {
                return BaseResult.Failure(ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
            }

            foreach (var item in order.Items)
            {
                if (item.Product == null)
                {
                    return BaseResult.Failure(ErrorCodes.ProductNotFound,
                        string.Format(ErrorMessage.ProductNotFound, item.Product.Id));
                }
            }

            order.ShipOrder();

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }

        public async Task<BaseResult> UpdateBulkOrderItemsAsync(long orderId, List<UpdateOrderItemDto> updateDtos, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
            if (order == null)
            {
                return BaseResult.Failure(ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
            }

            var productIds = updateDtos.Select(a => a.ProductId).Distinct().ToList();

            var products = await _productRepository.GetQueryable()
                .Where(p => productIds.Contains(p.Id))
                .AsNoTracking()
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            foreach (var update in updateDtos)
            {
                if (!products.TryGetValue(update.ProductId, out var product))
                {
                    return BaseResult.Failure(ErrorCodes.ProductNotFound,
                        string.Format(ErrorMessage.ProductNotFound, update.ProductId));
                }
                order.UpdateOrderItem(update.ProductId, update.NewQuantity, product.Price, product);
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }

        public async Task<BaseResult> UpdateStatusAsync(long orderId, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
            if (order == null)
            {
                return BaseResult.Failure(ErrorCodes.OrderNotFound, ErrorMessage.OrderNotFound);
            }

            order.UpdateStatus(dto.NewStatus);

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }
    }
}
