using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Resources;
using OrderPaymentSystem.DAL;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Interfaces.Validations;
using OrderPaymentSystem.Domain.Result;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace OrderPaymentSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IProductValidator _productValidator;
        private readonly ILogger _logger;

        public ProductService(IBaseRepository<Product> productRepository,
            IMapper mapper, IProductValidator productValidator, ILogger logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _productValidator = productValidator;
            _logger = logger;
        }


        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            try
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

                return new BaseResult<ProductDto>()
                {
                    Data = _mapper.Map<ProductDto>(product),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ProductDto>> DeleteProductAsync(int id)
        {
            try
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

                return new BaseResult<ProductDto>()
                {
                    Data = _mapper.Map<ProductDto>(product),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }
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
                _logger.Warning($"Продукт с {id} не найден", id);
                return Task.FromResult(new BaseResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.ProductNotFound,
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                });
            }

            return Task.FromResult(new BaseResult<ProductDto>()
            {
                Data = product,
            });
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<ProductDto>> GetProductsAsync()
        {
            ProductDto[] products;
            try
            {
                products = await _productRepository.GetAll()
                    .Select(x => new ProductDto(x.Id, x.ProductName, x.Description, x.Cost, x.CreatedAt.ToLongDateString()))
                    .ToArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new CollectionResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }

            if (!products.Any())
            {
                _logger.Warning(ErrorMessage.ProductsNotFound, products.Length);
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
            try
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
                product.ProductName = dto.ProductName;//Проверить работает ли здесь мапинг
                product.Description = dto.Description;
                product.Cost = dto.Cost;

                var updatedProduct = _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();

                return new BaseResult<ProductDto>()
                {
                    Data = _mapper.Map<ProductDto>(updatedProduct),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ProductDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }
        }
    }
}
