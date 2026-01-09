using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

public class BasketItemService : IBasketItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BasketItemService(IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DataResult<BasketItemDto>> CreateAsync(Guid userId, CreateBasketItemDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId, asNoTracking: true, cancellationToken);
        if (product == null)
        {
            return DataResult<BasketItemDto>.Failure(DomainErrors.Product.NotFound(dto.ProductId));
        }

        var basketItem = BasketItem.Create(userId, product.Id, dto.Quantity, product);

        await _unitOfWork.BasketItems.CreateAsync(basketItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<BasketItemDto>.Success(_mapper.Map<BasketItemDto>(basketItem));
    }

    public async Task<BaseResult> DeleteByIdAsync(long basketItemId, CancellationToken cancellationToken = default)
    {
        var basketItem = await _unitOfWork.BasketItems.GetByIdAsync(basketItemId, cancellationToken);
        if (basketItem == null)
        {
            return BaseResult.Failure(DomainErrors.BasketItem.NotFound(basketItemId));
        }

        _unitOfWork.BasketItems.Remove(basketItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }

    public async Task<CollectionResult<BasketItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.BasketItems.GetByUserIdQuery(userId)
            .AsProjected<BasketItem, BasketItemDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<BasketItemDto>.Success(items);
    }

    public async Task<DataResult<BasketItemDto>> UpdateQuantityAsync(long basketItemId, UpdateQuantityDto dto, CancellationToken cancellationToken = default)
    {
        var basketItem = await _unitOfWork.BasketItems.GetByIdWithProductAsync(basketItemId, cancellationToken);
        if (basketItem == null)
        {
            return DataResult<BasketItemDto>.Failure(DomainErrors.BasketItem.NotFound(basketItemId));
        }

        basketItem.UpdateQuantity(dto.NewQuantity, basketItem.Product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return DataResult<BasketItemDto>.Success(_mapper.Map<BasketItemDto>(basketItem));
    }
}
