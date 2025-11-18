using Moq;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.UnitTests.Configurations;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ProductTests;

public class GetProductsTests : IClassFixture<ProductServiceFixture>
{
    private readonly ProductServiceFixture _productServiceFixture;

    public GetProductsTests(ProductServiceFixture productServiceFixture)
    {
        _productServiceFixture = productServiceFixture;
    }

    private readonly ProductServiceFixture _fixture = new();

    [Fact]
    public async Task GetProductsAsync_ProductsFound_ShouldReturnProductDtosFromCache()
    {
        var products = _fixture.GetProductDtos();

        _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
            .ReturnsAsync(products);

        // Act
        var result = await _fixture.ProductService.GetProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldFetchProductsFromDatabaseAndCache()
    {
        // Arrange
        var products = _fixture.GetProductDtos();

        _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
            .ReturnsAsync((ProductDto[])null);
        _fixture.MediatorMock.Setup(x => x.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _fixture.ProductService.GetProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Count);
        Assert.IsAssignableFrom<IEnumerable<ProductDto>>(result.Data);
    }

    [Fact]
    public async Task GetProducts_ProductsNotFound_ReturnsErrorResult()
    {
        //Arrange
        _fixture.CacheServiceMock.Setup(x => x.GetObjectAsync<ProductDto[]>(It.IsAny<string>()))
            .ReturnsAsync((ProductDto[])null);

        // Act
        var result = await _fixture.ProductService.GetProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error.Code, (int)ErrorCodes.ProductsNotFound);
    }
}
