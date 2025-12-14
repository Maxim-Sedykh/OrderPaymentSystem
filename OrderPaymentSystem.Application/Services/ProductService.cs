using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
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
    private readonly IMediator _mediator;
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
        IMediator mediator,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _mediator = mediator;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<BaseResult> CreateProductAsync(CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.GetQueryable()
            .AsNoTracking()
            .AnyAsync(x => x.ProductName == dto.ProductName, cancellationToken);

        if (productExists)
        {
            return BaseResult.Failure((int)ErrorCodes.ProductAlreadyExist, ErrorMessage.ProductAlreadyExist);
        }

        var product = new Product()
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Cost = dto.Cost,
        };

        await _productRepository.CreateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created successfully: {ProductName} (ID: {ProductId})",
            dto.ProductName, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<BaseResult> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product == null)
        {
            return BaseResult.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product deleted successfully: {ProductName} (ID: {ProductId})",
            product.ProductName, product.Id);

        return BaseResult.Success();
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetQueryable()
            .Where(x => x.Id == id)
            .AsProjected<Product, ProductDto>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        return DataResult<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetQueryable()
            .AsProjected<Product, ProductDto>(_mapper)
            .ToArrayAsync(cancellationToken);

        if (products.Length == 0)
        {
            _logger.LogError("No products found in database");

            return CollectionResult<ProductDto>.Failure((int)ErrorCodes.ProductsNotFound, ErrorMessage.ProductsNotFound);
        }

        return CollectionResult<ProductDto>.Success(products);
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> UpdateProductAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        if (!HasProductChanges(product, dto))
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
        }

        UpdateProductProperties(product, dto);
        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var updatedProduct = _mapper.Map<ProductDto>(product);

        return DataResult<ProductDto>.Success(updatedProduct);
    }

    /// <summary>
    /// Проверяет наличие изменений в данных продукта
    /// </summary>
    /// <param name="product">Текущий продукт</param>
    /// <param name="dto">DTO с новыми данными</param>
    /// <returns>True если есть изменения, иначе false</returns>
    private static bool HasProductChanges(Product product, UpdateProductDto dto)
    {
        return product.ProductName != dto.ProductName ||
               product.Description != dto.Description ||
               product.Cost != dto.Cost;
    }

    /// <summary>
    /// Обновляет свойства продукта на основе DTO
    /// </summary>
    /// <param name="product">Продукт для обновления</param>
    /// <param name="dto">DTO с новыми значениями</param>
    private static void UpdateProductProperties(Product product, UpdateProductDto dto)
    {
        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.Cost = dto.Cost;
    }
}
