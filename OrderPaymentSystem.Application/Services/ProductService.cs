using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Cache;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;


namespace OrderPaymentSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;
        private readonly ICacheService _cacheService;

        public ProductService(IBaseRepository<Product> productRepository,
            IMapper mapper,
            IMessageProducer messageProducer,
            IOptions<RabbitMqSettings> rabbitMqOptions,
            ICacheService cacheService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
            _cacheService = cacheService;
        }

        
        /// <inheritdoc/>
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

            product = _mapper.Map<Product>(dto);

            await _productRepository.CreateAsync(product);
            await _productRepository.SaveChangesAsync();

            _messageProducer.SendMessage(product, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<ProductDto>()
            {
                Data = _mapper.Map<ProductDto>(product),
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

            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync();

            _messageProducer.SendMessage(product, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            var result = new ProductDto()
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Cost = product.Cost,
                CreatedAt = product.CreatedAt.ToLongDateString(),
            };

            return new BaseResult<ProductDto>()
            {
                Data = _mapper.Map<ProductDto>(product)
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _cacheService.GetObjectAsync(
                $"product:{id}",
                async () =>
                {
                    return await _productRepository.GetAll()
                        .AsNoTracking()
                        .Where(x => x.Id == id)
                        .Select(x => _mapper.Map<ProductDto>(x))
                        .SingleOrDefaultAsync();
                });

            if (product == null)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.ProductNotFound,
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }

            _messageProducer.SendMessage(product, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<ProductDto>()
            {
                Data = product,
            };
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<ProductDto>> GetProductsAsync()
        {
            var products = await _cacheService.GetObjectAsync(
                "products",
                async () =>
                {
                    return await _productRepository.GetAll().AsNoTracking()
                        .Select(x => _mapper.Map<ProductDto>(x))
                        .ToListAsync();
                });

            if (!products.Any())
            {
                return new CollectionResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.ProductsNotFound,
                    ErrorCode = (int)ErrorCodes.ProductsNotFound
                };
            }

            _messageProducer.SendMessage(products, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new CollectionResult<ProductDto>()
            {
                Data = products,
                Count = products.Count
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
                product.ProductName = dto.ProductName;
                product.Description = dto.Description;
                product.Cost = dto.Cost;

                var updatedProduct = _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();

                _messageProducer.SendMessage(product, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

                return new BaseResult<ProductDto>()
                {
                    Data = _mapper.Map<ProductDto>(updatedProduct),
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
