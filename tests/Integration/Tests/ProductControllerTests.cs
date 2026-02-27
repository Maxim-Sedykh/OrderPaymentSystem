using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.IntegrationTests.Base;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Tests;

/// <summary>
/// Тестирование взаимодействия с товарами.
/// </summary>
public class ProductControllerTests : BaseIntegrationTest
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="factory">Фабрика теста.</param>
    public ProductControllerTests(IntegrationTestFactory factory) : base(factory) { }

    /// <summary>
    /// Создание товара в качестве пользователем с ролью Admin должно быть успехом.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Удаление товара обычным пользователем должно быть запрещено
    /// </summary>
    /// <returns></returns>
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
