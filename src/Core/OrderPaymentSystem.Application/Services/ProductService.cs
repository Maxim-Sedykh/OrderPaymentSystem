using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Interfaces.Cache;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Specifications;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис отвечающий за работу с товарами (Product)
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly ICacheService _cacheService;

    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<BaseResult> CreateAsync(CreateProductDto dto,
        CancellationToken ct = default)
    {
        var productExists = await _unitOfWork.Products.AnyAsync(ProductSpecs.ByName(dto.Name), ct);
        if (productExists)
        {
            return BaseResult.Failure(DomainErrors.Product.AlreadyExist(dto.Name));
        }

        var product = Product.Create(dto.Name, dto.Description, dto.Price, dto.StockQuantity);

        await _unitOfWork.Products.CreateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.Product.All, ct);

        _logger.LogInformation("Product created successfully: {ProductName} (ID: {ProductId})",
            dto.Name, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.Products.GetFirstOrDefaultAsync(ProductSpecs.ById(id), ct);

        if (product == null)
        {
            return BaseResult.Failure(DomainErrors.Product.NotFound(id));
        }

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.Product.All, ct);
        await _cacheService.RemoveAsync(CacheKeys.Product.ById(id), ct);

        _logger.LogInformation("Product deleted successfully: {ProductName} (ID: {ProductId})",
            product.Name, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _cacheService.GetOrCreateAsync(CacheKeys.Product.ById(id),
            async (token) => await _unitOfWork.Products.GetProjectedAsync<ProductDto>(ProductSpecs.ById(id), token),
            ct: ct);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure(DomainErrors.Product.NotFound(id));
        }

        return DataResult<ProductDto>.Success(product);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        return CollectionResult<ProductDto>.Success(
            await _cacheService.GetOrCreateAsync(CacheKeys.Product.All,
                async (token) => await _unitOfWork.Products.GetListProjectedAsync<ProductDto>(ct: token),
                ct: ct));
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _unitOfWork.Products.GetFirstOrDefaultAsync(ProductSpecs.ById(id), ct);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure(DomainErrors.Product.NotFound(id));
        }

        product.UpdateDetails(dto.Name, dto.Description, dto.Price, dto.StockQuantity);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync(CacheKeys.Product.All, ct);
        await _cacheService.RemoveAsync(CacheKeys.Product.ById(id), ct);

        var updatedProduct = _mapper.Map<ProductDto>(product);

        return DataResult<ProductDto>.Success(updatedProduct);
    }
}
