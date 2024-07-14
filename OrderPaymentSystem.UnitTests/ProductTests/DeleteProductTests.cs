using MockQueryable.Moq;
using Moq;
using OrderPaymentSystem.Application.Commands;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.UnitTests.Configurations;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests
{
    public class DeleteProductTests : IClassFixture<ProductServiceFixture>
    {
        private readonly ProductServiceFixture _fixture = new ProductServiceFixture();

        [Fact]
        public async Task DeleteProductAsync_ShouldDeleteProductAndSendMessage()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId };
            var productDto = new ProductDto { Id = productId };

            _fixture.ProductRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Product> { product }.AsQueryable().BuildMockDbSet().Object);
            _fixture.MapperMock.Setup(x => x.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _fixture.ProductService.DeleteProductAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(productDto, result.Data);
            _fixture.MediatorMock.Verify(x => x.Send(It.Is<DeleteProductCommand>(cmd => cmd.Product.Id == productId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ReturnsErrorResult()
        {
            // Arrange
            var productId = 0; // Товара с таким идентификатором не должно существовать

            // Act
            var result = await _fixture.ProductService.DeleteProductAsync(productId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)ErrorCodes.ProductNotFound, result.ErrorCode);
        }
    }
}
