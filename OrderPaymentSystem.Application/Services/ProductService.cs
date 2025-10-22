using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Commands;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using Serilog;


namespace OrderPaymentSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ProductService(IBaseRepository<Product> productRepository,
            IMapper mapper,
            ICacheService cacheService,
            IMediator mediator,
            ILogger logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _mediator = mediator;
            _logger = logger;
        }


        public async Task<BaseResult<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.ProductName == dto.ProductName);

            if (product != null)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorCode = (int)ErrorCodes.ProductAlreadyExist,
                    ErrorMessage = ErrorMessage.ProductAlreadyExist
                };
            }

            ProductDto createdProduct = await _mediator.Send(new CreateProductCommand(dto.ProductName, dto.Description, dto.Cost));

            return new BaseResult<ProductDto>()
            {
                Data = createdProduct,
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorCode = (int)ErrorCodes.ProductNotFound,
                    ErrorMessage = ErrorMessage.ProductNotFound
                };
            }

            await _mediator.Send(new DeleteProductCommand(product));

            return new BaseResult<ProductDto>()
            {
                Data = _mapper.Map<ProductDto>(product)
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _cacheService.GetObjectAsync<ProductDto>(string.Format(CacheKeys.Product, id));

            if (product == null)
            {
                try
                {
                    product = await _mediator.Send(new GetProductByIdQuery(id), new CancellationToken());
                    await _cacheService.SetObjectAsync(string.Format(CacheKeys.Product, id), product);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    return new BaseResult<ProductDto>()
                    {
                        ErrorMessage = ErrorMessage.InternalServerError,
                        ErrorCode = (int)ErrorCodes.InternalServerError
                    };
                }
            }

            if (product == null)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.ProductNotFound,
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }

            return new BaseResult<ProductDto>()
            {
                Data = product,
            };
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<ProductDto>> GetProductsAsync()
        {
            var products = await _cacheService.GetObjectAsync<ProductDto[]>(CacheKeys.Products);

            if (products == null)
            {
                try
                {
                    products = await _mediator.Send(new GetProductsQuery(), new CancellationToken());

                    await _cacheService.SetObjectAsync(CacheKeys.Products, products);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    return new CollectionResult<ProductDto>()
                    {
                        ErrorMessage = ErrorMessage.InternalServerError,
                        ErrorCode = (int)ErrorCodes.InternalServerError
                    };
                }
            }

            if (!products.Any())
            {
                _logger.Error(ErrorMessage.ProductsNotFound, products.Length);
                return new CollectionResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.ProductsNotFound,
                    ErrorCode = (int)ErrorCodes.ProductsNotFound
                };
            }

            return new CollectionResult<ProductDto>()
            {
                Data = products,
                Count = products.Length
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (product == null)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorCode = (int)ErrorCodes.ProductNotFound,
                    ErrorMessage = ErrorMessage.ProductNotFound
                };
            }

            if (product.ProductName != dto.ProductName
                || product.Description != dto.Description
                || product.Cost != dto.Cost)
            {
                await _mediator.Send(new UpdateProductCommand(dto.ProductName, dto.Description, dto.Cost, product));

                return new BaseResult<ProductDto>()
                {
                    Data = _mapper.Map<ProductDto>(product),
                };
            }

            return new BaseResult<ProductDto>()
            {
                ErrorMessage = ErrorMessage.NoChangesFound,
                ErrorCode = (int)ErrorCodes.NoChangesFound
            };
        }
    }
}
