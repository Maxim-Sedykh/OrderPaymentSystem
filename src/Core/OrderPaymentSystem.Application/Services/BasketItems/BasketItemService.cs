using MapsterMapper;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services.BasketItems;

internal class BasketItemService : IBasketItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BasketItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DataResult<BasketItemDto>> CreateAsync(Guid userId, CreateBasketItemDto dto, CancellationToken ct = default)
    {
        var product = await _unitOfWork.Products.GetFirstOrDefaultAsync(ProductSpecs.ByIdNoTracking(dto.ProductId), ct);
        if (product == null)
        {
            return DataResult<BasketItemDto>.Failure(DomainErrors.Product.NotFound(dto.ProductId));
        }

        var basketItem = BasketItem.Create(userId, product.Id, dto.Quantity, product);

        await _unitOfWork.BasketItems.CreateAsync(basketItem, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<BasketItemDto>.Success(_mapper.Map<BasketItemDto>(basketItem));
    }

    public async Task<BaseResult> DeleteByIdAsync(long basketItemId, CancellationToken ct = default)
    {
        var basketItem = await _unitOfWork.BasketItems.GetFirstOrDefaultAsync(BasketItemSpecs.ById(basketItemId), ct);
        if (basketItem == null)
        {
            return BaseResult.Failure(DomainErrors.BasketItem.NotFound(basketItemId));
        }

        _unitOfWork.BasketItems.Remove(basketItem);
        await _unitOfWork.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    public async Task<CollectionResult<BasketItemDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var items = await _unitOfWork.BasketItems
            .GetListProjectedAsync<BasketItemDto>(BasketItemSpecs.ByUserId(userId), ct);

        return CollectionResult<BasketItemDto>.Success(items);
    }

    public async Task<DataResult<BasketItemDto>> UpdateQuantityAsync(long basketItemId, UpdateQuantityDto dto, CancellationToken ct = default)
    {
        var basketItem = await _unitOfWork.BasketItems.GetFirstOrDefaultAsync(BasketItemSpecs.ById(basketItemId).WithProduct(), ct);
        if (basketItem == null)
        {
            return DataResult<BasketItemDto>.Failure(DomainErrors.BasketItem.NotFound(basketItemId));
        }

        basketItem.UpdateQuantity(dto.NewQuantity, basketItem.ProductId, basketItem.Product);
        await _unitOfWork.SaveChangesAsync(ct);

        return DataResult<BasketItemDto>.Success(_mapper.Map<BasketItemDto>(basketItem));
    }
}
