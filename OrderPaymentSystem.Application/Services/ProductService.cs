using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Commands.ProductCommands;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using Serilog;

namespace OrderPaymentSystem.Application.Services;

/// <summary>
/// Сервис отвечающий за работу с доменной части товаров (Product)
/// </summary>
/// <param name="productRepository">Репозиторий для работы с товарами</param>
/// <param name="mapper">Маппер</param>
/// <param name="mediator">Медиатр</param>
/// <param name="logger">Логгер</param>
public class ProductService(IBaseRepository<Product> productRepository,
    IMapper mapper,
    IMediator mediator,
    ILogger logger) : IProductService
{
    public async Task<DataResult<ProductDto>> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetQueryable().FirstOrDefaultAsync(x => x.ProductName == dto.ProductName, cancellationToken);

        if (product != null)
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.ProductAlreadyExist, ErrorMessage.ProductAlreadyExist);
        }

        var createdProduct = await mediator.Send(new CreateProductCommand(dto.ProductName, dto.Description, dto.Cost), cancellationToken);

        return DataResult<ProductDto>.Success(createdProduct);
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        await mediator.Send(new DeleteProductCommand(product), cancellationToken);

        return DataResult<ProductDto>.Success(mapper.Map<ProductDto>(product));
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);
        }

        if (product == null)
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        return DataResult<ProductDto>.Success(mapper.Map<ProductDto>(product));
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await mediator.Send(new GetProductsQuery(), cancellationToken);

        if (products == null)
        {
            return CollectionResult<ProductDto>.Failure((int)ErrorCodes.InternalServerError, ErrorMessage.InternalServerError);
        }

        if (products.Length == 0)
        {
            logger.Error(ErrorMessage.ProductsNotFound);

            return CollectionResult<ProductDto>.Failure((int)ErrorCodes.ProductsNotFound, ErrorMessage.ProductsNotFound);
        }

        return CollectionResult<ProductDto>.Success(products);
    }

    /// <inheritdoc/>
    public async Task<DataResult<ProductDto>> UpdateProductAsync(UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (product == null)
        {
            return DataResult<ProductDto>.Failure((int)ErrorCodes.ProductNotFound, ErrorMessage.ProductNotFound);
        }

        if (product.ProductName != dto.ProductName
            || product.Description != dto.Description
            || product.Cost != dto.Cost)
        {
            await mediator.Send(new UpdateProductCommand(dto.ProductName, dto.Description, dto.Cost, product), cancellationToken);

            return DataResult<ProductDto>.Success(mapper.Map<ProductDto>(product));
        }

        return DataResult<ProductDto>.Failure((int)ErrorCodes.NoChangesFound, ErrorMessage.NoChangesFound);
    }
}
