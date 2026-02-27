using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.IntegrationTests.Base;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Tests;

/// <summary>
/// Тестирование функционала с элементами корзины.
/// </summary>
public class BasketIsolationTests : BaseIntegrationTest
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="factory">Фабрика для создания зависимостей.</param>
    public BasketIsolationTests(IntegrationTestFactory factory) : base(factory) { }

    /// <summary>
    /// Пользователь должен видеть только свои элементы корзины
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task User_Should_Only_See_Their_Own_Basket()
    {
        // Arrange
        await AuthenticateAsync(TestConstants.AdminLogin, DefaultRoles.Admin);

        var productDto = new CreateProductDto("Test Isolation Product", "Desc", 500m, 5);
        var createdProduct = await Factory.CreateProductAsync(productDto, Client);
        createdProduct.Should().NotBeNull();

        var itemDto = new CreateBasketItemDto(createdProduct.Id, 4);
        var basketAddResponse = await Client.PostAsJsonAsync(TestConstants.ApiBasketV1, itemDto);
        basketAddResponse.EnsureSuccessStatusCode();

        var basketResponseUser1 = await Client.GetAsync(TestConstants.ApiBasketV1);
        basketResponseUser1.EnsureSuccessStatusCode();
        var basketItemsUser1 = await basketResponseUser1.Content.ReadFromJsonAsync<List<BasketItemDto>>();
        basketItemsUser1.Should().ContainSingle().And.Contain(item => item.ProductId == 1 && item.Quantity == 4);

        // Act
        await AuthenticateAsync(TestConstants.CommonUserLogin, DefaultRoles.User, TestConstants.CommonUserPassword);

        // Assert
        var basketResponseUser2 = await Client.GetAsync(TestConstants.ApiBasketV1);
        basketResponseUser2.EnsureSuccessStatusCode();
        var basketItemsUser2 = await basketResponseUser2.Content.ReadFromJsonAsync<List<BasketItemDto>>();
        basketItemsUser2.Should().BeEmpty();
    }
}
