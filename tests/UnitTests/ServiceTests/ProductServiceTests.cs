using FluentAssertions;
using Moq;
using OrderPaymentSystem.Application.Constants;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Specifications;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

public class ProductServiceTests
{
    private readonly ProductFixture _fixture;

    public ProductServiceTests()
    {
        _fixture = new ProductFixture();
    }

    [Fact]
    public async Task GetByIdAsync_WhenInCache_ShouldReturnFromCacheAndNotTouchDb()
    {
        // Arrange
        var productId = 1;
        var cachedProduct = TestDataFactory.CreateProductDto(productId);

        _fixture.SetupCacheGet(CacheKeys.Product.ById(productId), cachedProduct);

        // Act
        var result = await _fixture.Service.GetByIdAsync(productId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(cachedProduct);
        _fixture.ProductRepo.Verify(r => r.GetProjectedAsync<ProductDto>(It.IsAny<BaseSpecification<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldSaveAndInvalidateGlobalCache()
    {
        // Arrange
        var dto = new CreateProductDto("iPhone 15", "Phone", 999m, 10);
        _fixture.SetupProductExistence(false);

        // Act
        var result = await _fixture.Service.CreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fixture.VerifySaved();
        _fixture.VerifyCacheRemoved(CacheKeys.Product.All);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_ShouldUpdateAndInvalidateSpecificCache()
    {
        // Arrange
        var product = TestDataFactory.Product.WithId(1).Build();
        var updateDto = new UpdateProductDto { Name = "New Name", Price = 500m };

        _fixture.SetupProduct(product)
                .SetupMapping<Product, ProductDto>(TestDataFactory.CreateProductDto(product.Id, "New Name"));

        // Act
        var result = await _fixture.Service.UpdateAsync(product.Id, updateDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be("New Name");
        _fixture.VerifySaved();
        _fixture.VerifyCacheRemoved(CacheKeys.Product.All);
        _fixture.VerifyCacheRemoved(CacheKeys.Product.ById(product.Id));
    }

    [Fact]
    public async Task DeleteByIdAsync_WhenExists_ShouldRemoveAndInvalidateCache()
    {
        // Arrange
        var product = TestDataFactory.Product.WithId(1).Build();
        _fixture.SetupProduct(product);

        // Act
        var result = await _fixture.Service.DeleteByIdAsync(product.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fixture.ProductRepo.Verify(r => r.Remove(product), Times.Once);
        _fixture.VerifySaved();
        _fixture.VerifyCacheRemoved(CacheKeys.Product.ById(product.Id));
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFoundInCacheAndDb_ShouldReturnError()
    {
        // Arrange
        _fixture.SetupCacheGet<ProductDto>(CacheKeys.Product.ById(99), null!);

        // Act
        var result = await _fixture.Service.GetByIdAsync(99);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Product.NotFound(99));
    }
}
