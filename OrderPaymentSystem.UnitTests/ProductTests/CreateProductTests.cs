using Moq;
using OrderPaymentSystem.Application.Commands.ProductCommands;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.UnitTests.Configurations;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests;

public class CreateProductTests : IClassFixture<ProductServiceFixture>
{
    private readonly ProductServiceFixture _fixture = new();

    [Fact]
    public async Task CreateProductAsync_ValidInput_ReturnsSuccessResultWithData()
    {
        // Arrange
        var dto = new CreateProductDto("ProductForTest", "Description of product for test", 999.99m);

        _fixture.MapperMock.Setup(mapper => mapper.Map<Product>(dto)).Returns(new Product());
        _fixture.MapperMock.Setup(mapper => mapper.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto());

        _fixture.MediatorMock.Setup(x => x.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto());

        // Act
        var result = await _fixture.ProductService.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.IsType<ProductDto>(result.Data);
        _fixture.MediatorMock.Verify(x => x.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_DuplicateProductName_ReturnsErrorResult()
    {
        // Arrange
        var dto = new CreateProductDto("Test Product #1", "Test description", 999.99m);

        _fixture.MapperMock.Setup(mapper => mapper.Map<Product>(dto)).Returns(new Product());

        // Act
        var result = await _fixture.ProductService.CreateAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.ProductAlreadyExist, result.Error.Code);
    }
}
