using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.IntegrationTests.Base;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Tests;

public class ProductControllerTests : BaseIntegrationTest
{
    public ProductControllerTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateProduct_AsAdmin_ShouldReturnCreated()
    {
        // Arrange
        await AuthenticateAsync("admin", DefaultRoles.Admin);
        var productDto = new CreateProductDto("Redmo xiaomi poco 7", "Coolest smartphone ever", 1000m, 50);

        // Act
        var response = await Client.PostAsJsonAsync(TestConstants.ApiProductsV1, productDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DeleteProduct_AsRegularUser_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateAsync("normal_user", DefaultRoles.User);

        // Act
        var response = await Client.DeleteAsync($"{TestConstants.ApiProductsV1}/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
