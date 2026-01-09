using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.Extensions;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис отвечающий за работу с доменной части товаров (Product)
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<BaseResult> CreateAsync(CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _unitOfWork.Products.ExistsByNameAsync(dto.Name, cancellationToken);
        if (productExists)
        {
            return BaseResult.Failure(DomainErrors.Product.AlreadyExist(dto.Name));
        }

        var product = Product.Create(dto.Name, dto.Description, dto.Price, dto.StockQuantity);

        await _unitOfWork.Products.CreateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created successfully: {ProductName} (ID: {ProductId})",
            dto.Name, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken: cancellationToken);

        if (product == null)
        {
            return BaseResult.Failure(DomainErrors.Product.NotFound(id));
        }

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product deleted successfully: {ProductName} (ID: {ProductId})",
            product.Name, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdQuery(id)
            .AsProjected<Product, ProductDto>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure(DomainErrors.Product.NotFound(id));
        }

        return DataResult<ProductDto>.Success(product);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllQuery()
            .AsProjected<Product, ProductDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<ProductDto>.Success(products);
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken: cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure(DomainErrors.Product.NotFound(id));
        }

        product.UpdateDetails(dto.Name, dto.Description, dto.Price, dto.StockQuantity);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProduct = _mapper.Map<ProductDto>(product);

        return DataResult<ProductDto>.Success(updatedProduct);
    }
}
