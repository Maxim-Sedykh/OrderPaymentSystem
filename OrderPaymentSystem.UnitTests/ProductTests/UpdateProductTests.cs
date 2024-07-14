using MockQueryable.Moq;
using Moq;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.UnitTests.Configurations;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class UpdateProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductServiceFixture _fixture = new ProductServiceFixture();

        [Fact]
        public async Task UpdateProductAsync_ValidInput_ReturnsSuccessResultWithData()
        {
            // Arrange
            var dto = new UpdateProductDto { Id = 1, ProductName = "Product name to update", Description = "Product description", Cost = 1000 };
            var updatedProduct = new Product { Id = 1, ProductName = "Old name", Description = "Old description", Cost = 500 };

            _fixture.ProductRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Product> { updatedProduct }.AsQueryable().BuildMockDbSet().Object);
            _fixture.MapperMock.Setup(mapper => mapper.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(new ProductDto { Id = dto.Id, ProductName = dto.ProductName, Description = dto.Description, Cost = dto.Cost });

            // Act
            var result = await _fixture.ProductService.UpdateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.IsType<ProductDto>(result.Data);
            Assert.Equal(dto.ProductName, result.Data.ProductName);
            Assert.Equal(dto.Description, result.Data.Description);
            Assert.Equal(dto.Cost, result.Data.Cost);
        }

        [Fact]
        public async Task UpdateProductAsync_NonExistingProduct_ReturnsErrorResultProductNotFound()
        {
            //Arrange
            var dto = new UpdateProductDto { Id = 2, ProductName = "Product name to update", Description = "Product description", Cost = 1000 };
            var updatedProduct = new Product { Id = 1, ProductName = "Old name", Description = "Old description", Cost = 500 };

            _fixture.ProductRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Product> { updatedProduct }.AsQueryable().BuildMockDbSet().Object);

            //Act
            var result = await _fixture.ProductService.UpdateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.ProductNotFound);
        }

        [Fact]
        public async Task UpdateProductAsync_NoChangesToUpdateInput_ReturnsErrorResultNoChangesFound()
        {
            // Arrange
            var dto = new UpdateProductDto { Id = 1, ProductName = "Product name to update", Description = "Product description", Cost = 1000 };
            var updatedProduct = new Product { Id = dto.Id, ProductName = dto.ProductName, Description = dto.Description, Cost = dto.Cost };

            _fixture.ProductRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Product> { updatedProduct }.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _fixture.ProductService.UpdateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(result.ErrorCode, (int)ErrorCodes.NoChangesFound);
        }
    }
}
