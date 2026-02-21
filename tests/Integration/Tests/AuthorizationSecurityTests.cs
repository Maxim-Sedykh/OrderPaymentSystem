using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.IntegrationTests.Base;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Tests;

public class AuthorizationSecurityTests : BaseIntegrationTest
{
    public AuthorizationSecurityTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateProduct_ByRegularUser_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateAsync(TestConstants.CommonUserLogin, TestConstants.UserRole, TestConstants.CommonUserPassword);

        // Act
        var createDto = new CreateProductDto("Hacker Tool", "Should fail", 999m, 1);
        var response = await Client.PostAsJsonAsync(TestConstants.ApiProductsV1, createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteProduct_AsRegularUser_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateAsync(TestConstants.AdminLogin, TestConstants.AdminRole);
        var productDto = new CreateProductDto("Test Product", "For Deletion", 100m, 10);
        var createdProduct = await Factory.CreateProductAsync(productDto, Client);
        createdProduct.Should().NotBeNull();

        // Act
        await AuthenticateAsync(TestConstants.CommonUserLogin, TestConstants.UserRole, TestConstants.CommonUserPassword);
        var response = await Client.DeleteAsync($"{TestConstants.ApiProductsV1}/{createdProduct!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateProduct_AsAdmin_ShouldReturnCreated()
    {
        // Arrange
        await AuthenticateAsync(TestConstants.AdminLogin, TestConstants.AdminRole);
        var productDto = new CreateProductDto("IPhone 17", "Pro max turbo", 1000m, 50);

        // Act
        var response = await Client.PostAsJsonAsync(TestConstants.ApiProductsV1, productDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct.Name.Should().Be(productDto.Name);
    }
}
