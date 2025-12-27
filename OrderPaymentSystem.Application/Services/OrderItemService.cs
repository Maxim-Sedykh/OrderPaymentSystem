using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Dto;
using OrderPaymentSystem.Domain.Dto.OrderItem;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Exceptions;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validators;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<OrderItem> _orderItemRepository;
        private readonly IOrderItemValidator _orderItemValidator;
        private readonly IMapper _mapper;

        public OrderItemService(IBaseRepository<Product> productRepository, IBaseRepository<Order> orderRepository, IBaseRepository<OrderItem> orderItemRepository, IOrderItemValidator orderItemValidator, IMapper mapper)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderItemValidator = orderItemValidator;
            _mapper = mapper;
        }

        public async Task<DataResult<OrderItemDto>> CreateAsync(CreateOrderItemDto dto, CancellationToken cancellationToken = default)
        {
            var (order, product) = await GetOrderAndProductAsync(dto.OrderId, dto.ProductId, cancellationToken);

            var validateUpdatingOrderResult = _orderItemValidator.ValidateUpdatingOrder(order, product);
            if (!validateUpdatingOrderResult.IsSuccess)
            {
                return DataResult<OrderItemDto>.Failure(validateUpdatingOrderResult.Error);
            }

            var orderItem = OrderItem.Create(product.Id, dto.Quantity, product.Price);

            order.Items.Add(orderItem);

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return DataResult<OrderItemDto>.Success(_mapper.Map<OrderItemDto>(orderItem));
        }

        public async Task<BaseResult> DeleteByIdAsync(long orderItemId, CancellationToken cancellationToken = default)
        {
            var orderItem = await _orderItemRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == orderItemId, cancellationToken);
            if (orderItem == null)
            {
                return BaseResult.Failure(5001, "Order item not found");
            }

            _orderItemRepository.Remove(orderItem);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }

        public async Task<CollectionResult<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            var orderItems = await _orderItemRepository.GetQueryable()
                .Where(x => x.OrderId == orderId)
                .AsProjected<OrderItem, OrderItemDto>(_mapper)
                .ToArrayAsync(cancellationToken);

            if (orderItems.Length == 0)
            {
                return CollectionResult<OrderItemDto>.Failure(5002, "Order items not found");
            }

            return CollectionResult<OrderItemDto>.Success(orderItems);
        }

        public async Task<DataResult<OrderItemDto>> UpdateQuantityAsync(long orderItemId, UpdateQuantityDto dto, CancellationToken cancellationToken = default)
        {
            var orderItem = await _orderItemRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == orderItemId, cancellationToken);
            if (orderItem == null)
            {
                return DataResult<OrderItemDto>.Failure(5001, $"Order item with ID '{orderItemId}' does not exist.");
            }

            orderItem.UpdateQuantity(dto.NewQuantity);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);

            return DataResult<OrderItemDto>.Success(_mapper.Map<OrderItemDto>(orderItem));
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
    }
}
