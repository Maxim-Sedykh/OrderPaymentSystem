using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public OrderService(IMapper mapper, IBaseRepository<Order> orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<BaseResult> CompleteProcessingAsync(long orderId, long paymentId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
            if (order == null)
            {
                return BaseResult.Failure(1001, $"Order with ID '{orderId}' not found.");
            }

            var payment = await _paymentRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
            if (payment == null)
            {
                return BaseResult.Failure(1001, $"Payment with ID '{paymentId}' not found.");
            }

            if (payment.OrderId != orderId)
            {
                return BaseResult.Failure(1001, $"Payment {paymentId} is not associated with order {orderId}.");
            }

            order.AssignPayment(paymentId);
            order.ConfirmOrder();

            _orderRepository.Update(order);

            await _orderRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }

        public async Task<DataResult<OrderDto>> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default)
        {
            var orderItemEntities = dto.OrderItems
                .Select(x => OrderItem.Create(x.ProductId, x.Quantity, x.ProductPrice)).ToArray();

            var totalAmount = orderItemEntities.Sum(item => item.ItemTotalSum);
            var order = Order.Create(userId, dto.DeliveryAddress, orderItemEntities, totalAmount);

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

            if (orders == null)
            {
                return CollectionResult<OrderDto>.Failure(ErrorCodes.OrdersNotFound, ErrorMessage.OrdersNotFound);
            }

            return CollectionResult<OrderDto>.Success(orders);
        }

        public async Task<BaseResult> ShipOrderAsync(long orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
            if (order == null)
            {
                return BaseResult.Failure(666, $"Order with ID '{orderId}' not found.");
            }

            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);

                if (product == null)
                {
                    return BaseResult.Failure(666, $"Product with ID '{item.ProductId}' for order '{orderId}' not found.");
                }
                if (product.StockQuantity < item.Quantity)
                {
                    return BaseResult.Failure(666, $"Not enough stock for product '{product.Id}' to ship order '{orderId}'. Available: {product.StockQuantity}, Required: {item.Quantity}.");
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
                return BaseResult.Failure(666, $"Order with ID '{orderId}' not found.");
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
                    return BaseResult.Failure(666, $"Product with ID '{update.ProductId}' not found.");
                }
                if (update.NewQuantity != 0)
                {
                    order.UpdateOrderItems(update.ProductId, update.NewQuantity, product.Price);
                }
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

            await _orderRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }
    }
}
