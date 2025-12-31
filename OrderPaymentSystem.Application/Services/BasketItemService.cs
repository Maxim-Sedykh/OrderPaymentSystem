using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto;
using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services
{
    public class BasketItemService : IBasketItemService
    {
        private readonly IBaseRepository<BasketItem> _basketItemRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public BasketItemService(IBaseRepository<BasketItem> basketItemRepository,
            IBaseRepository<Product> productRepository,
            IMapper mapper)
        {
            _basketItemRepository = basketItemRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<DataResult<BasketItemDto>> CreateAsync(Guid userId, CreateBasketItemDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == dto.ProductId, cancellationToken);
            if (product == null)
            {
                return DataResult<BasketItemDto>.Failure(ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
            }

            var basketItem = BasketItem.Create(userId, product.Id, dto.Quantity, product);

            await _basketItemRepository.CreateAsync(basketItem, cancellationToken);
            await _basketItemRepository.SaveChangesAsync(cancellationToken);

            return DataResult<BasketItemDto>.Success(_mapper.Map<BasketItemDto>(basketItem));
        }

        public async Task<BaseResult> DeleteByIdAsync(long basketItemId, CancellationToken cancellationToken = default)
        {
            var basketItem = await _basketItemRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == basketItemId, cancellationToken);
            if (basketItem == null)
            {
                return BaseResult.Failure(
                    ErrorCodes.BasketItemNotFound,
                    string.Format(ErrorMessage.BasketItemNotFound, basketItemId));
            }

            _basketItemRepository.Remove(basketItem);
            await _basketItemRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }

        public async Task<CollectionResult<BasketItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var items = await _basketItemRepository.GetQueryable()
                .Where(x => x.UserId == userId)
                .AsProjected<BasketItem, BasketItemDto>(_mapper)
                .ToArrayAsync(cancellationToken);

            return CollectionResult<BasketItemDto>.Success(items);
        }

        public async Task<DataResult<BasketItemDto>> UpdateQuantityAsync(long basketItemId, UpdateQuantityDto dto, CancellationToken cancellationToken = default)
        {
            var basketItem = await _basketItemRepository.GetQueryable()
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == basketItemId, cancellationToken);
            if (basketItem == null)
            {
                return DataResult<BasketItemDto>.Failure(ErrorCodes.BasketItemNotFound, ErrorMessage.BasketItemNotFound);
            }

            basketItem.UpdateQuantity(dto.NewQuantity, basketItem.Product);
            await _basketItemRepository.SaveChangesAsync(cancellationToken);

            return DataResult<BasketItemDto>.Success(_mapper.Map<BasketItemDto>(basketItem));
        }
    }
}
