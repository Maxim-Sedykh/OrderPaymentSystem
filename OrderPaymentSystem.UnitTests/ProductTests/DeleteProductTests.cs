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
    public class DeleteProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductService _productService;
        private readonly Mock<IBaseRepository<Product>> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public DeleteProductTests(ProductServiceFixture fixture)
        {
            _productService = fixture.ProductService;
            _productRepositoryMock = fixture.ProductRepositoryMock;
            _mapperMock = fixture.MapperMock;
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_ReturnsSuccessResultWithData()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, ProductName = "TestProduct" };

            _productRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(new List<Product> { product }.AsQueryable().BuildMockDbSet().Object);
            _mapperMock.Setup(mapper => mapper.Map<Product>(product)).Returns(new Product());
            _mapperMock.Setup(mapper => mapper.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto());

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);

            _productRepositoryMock.Verify(repo => repo.Remove(product), Times.Once);
            _productRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ReturnsErrorResult()
        {
            // Arrange
            var productId = 1;

            _productRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(new List<Product>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)ErrorCodes.ProductNotFound, result.ErrorCode);
        }
    }
}
