using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using OrderPaymentSystem.Domain.Settings;
using OrderPaymentSystem.Producer.Interfaces;
using Serilog;


namespace OrderPaymentSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IProductValidator _productValidator;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;

        public ProductService(IBaseRepository<Product> productRepository,
            IMapper mapper, IProductValidator productValidator, ILogger logger,
            IMessageProducer messageProducer, IOptions<RabbitMqSettings> rabbitMqOptions)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _productValidator = productValidator;
            _messageProducer = messageProducer;
            _rabbitMqOptions = rabbitMqOptions;
        }


        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.ProductName == dto.ProductName);

            var result = _productValidator.CreateProductValidator(product);
            if (!result.IsSuccess)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
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
            var result = _productValidator.ValidateOnNull(product);

            if (!result.IsSuccess)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync();

            _messageProducer.SendMessage(product, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return new BaseResult<ProductDto>()
            {
                Data = _mapper.Map<ProductDto>(product),
            };
        }

        /// <inheritdoc/>
        public Task<BaseResult<ProductDto>> GetProductByIdAsync(int id)
        {
            ProductDto? product;
            product = _productRepository.GetAll()
                .Select(x => new ProductDto(x.Id, x.ProductName, x.Description, x.Cost, x.CreatedAt.ToLongDateString()))
                .AsEnumerable()
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return Task.FromResult(new BaseResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.ProductNotFound,
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                });
            }

            _messageProducer.SendMessage(product, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

            return Task.FromResult(new BaseResult<ProductDto>()
            {
                Data = product,
            });
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<ProductDto>> GetProductsAsync()
        {
            ProductDto[] products;

            products = await _productRepository.GetAll()
                .Select(x => new ProductDto(x.Id, x.ProductName, x.Description, x.Cost, x.CreatedAt.ToLongDateString()))
                .ToArrayAsync();

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
                Count = products.Length
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            var result = _productValidator.ValidateOnNull(product);

            if (!result.IsSuccess)
            {
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }
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
    }
}
