using AutoMapper;
using MockQueryable.Moq;
using Moq;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.UnitTests.Fixtures;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class CreateProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductService _productService;
        private readonly Mock<IBaseRepository<Product>> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public CreateProductTests(ProductServiceFixture fixture)
        {
            _productService = fixture.ProductService;
            _productRepositoryMock = fixture.ProductRepositoryMock;
            _mapperMock = fixture.MapperMock;
        }

        [Fact]
        public async Task CreateProductAsync_ValidInput_ReturnsSuccessResultWithData()
        {
            // Arrange
            var dto = new CreateProductDto("ProductForTest", "Description of product for test", 999.99m);

            _productRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Product>().AsQueryable().BuildMockDbSet().Object);
            _mapperMock.Setup(mapper => mapper.Map<Product>(dto)).Returns(new Product());
            _mapperMock.Setup(mapper => mapper.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto());

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.IsType<ProductDto>(result.Data);
        }

        [Fact]
        public async Task CreateProductAsync_DuplicateProductName_ReturnsErrorResult()
        {
            // Arrange
            var dto = new CreateProductDto("Алмазная мозаика", "Test description", 999.99m);

            var products = new List<Product>
            {
                new Product { ProductName = "Алмазная мозаика", Description = "test", Cost = 500 }
            }.AsQueryable();

            _productRepositoryMock.Setup(repo => repo.GetAll()).Returns(products.BuildMockDbSet().Object);
            _mapperMock.Setup(mapper => mapper.Map<Product>(dto)).Returns(new Product());

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)ErrorCodes.ProductAlreadyExist, result.ErrorCode);
        }
    }
}
