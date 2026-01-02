using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Databases.Repositories.Base;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис отвечающий за работу с доменной части товаров (Product)
/// </summary>
public class ProductService : IProductService
{
    private readonly IBaseRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="productRepository">Репозиторий для работы с товарами</param>
    /// <param name="mapper">Маппер</param>
    /// <param name="mediator">Посредник, медиатор</param>
    /// <param name="logger">Логгер</param>
    public ProductService(
        IBaseRepository<Product> productRepository,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<BaseResult> CreateAsync(CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.GetQueryable()
            .AnyAsync(x => x.Name == dto.Name, cancellationToken);

        if (productExists)
        {
            return BaseResult.Failure(ErrorCodes.ProductAlreadyExist, ErrorMessage.ProductAlreadyExist);
        }

        var product = Product.Create(dto.Name, dto.Description, dto.Price, dto.StockQuantity);

        await _productRepository.CreateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created successfully: {ProductName} (ID: {ProductId})",
            dto.Name, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product == null)
        {
            return BaseResult.Failure(ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product deleted successfully: {ProductName} (ID: {ProductId})",
            product.Name, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetQueryable()
            .Where(x => x.Id == id)
            .AsProjected<Product, ProductDto>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure(ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        return DataResult<ProductDto>.Success(product);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetQueryable()
            .AsProjected<Product, ProductDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<ProductDto>.Success(products);
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure(ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        product.UpdateDetails(dto.Name, dto.Description, dto.Price, dto.StockQuantity);

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var updatedProduct = _mapper.Map<ProductDto>(product);

        return DataResult<ProductDto>.Success(updatedProduct);
    }
}
