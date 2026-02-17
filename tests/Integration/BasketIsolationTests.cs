using FluentAssertions;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace OrderPaymentSystem.IntegrationTests
{
    public class BasketIsolationTests : BaseIntegrationTest
    {
        public BasketIsolationTests(IntegrationTestFactory factory) : base(factory) { }

        [Fact]
        public async Task User_Should_Only_See_Their_Own_Basket()
        {
            // 1. Arrange: Пользователь 1 добавляет товар в свою корзину
            await AuthenticateAsync("admin", "Admin");
            var productDto = new CreateProductDto("Test", "Desc", 500m, 5);
            var productCreateResponse = await Client.PostAsJsonAsync("/api/v1/products", productDto);
            var productCreateContent = await productCreateResponse.Content.ReadFromJsonAsync<ProductDto>();
            var itemDto = new CreateBasketItemDto(productCreateContent?.Id ?? 0, 1); // Предположим продукт 1 существует
            await Client.PostAsJsonAsync("/api/v1/basket", itemDto);

            var user1Response = await Client.GetAsync("/api/v1/basket");
            var basketItemsUser1 = await user1Response.Content.ReadFromJsonAsync<List<BasketItemDto>>();

            // 2. Act: Логинимся как Пользователь 2
            await AuthenticateAsync("user_two", "User");
            var user2Response = await Client.GetAsync("/api/v1/basket");
            var basketItemsUser2 = await user2Response.Content.ReadFromJsonAsync<List<BasketItemDto>>();

            // 3. Assert: Корзина пользователя 2 должна быть пустой
            basketItemsUser2.Should().BeEmpty();
            basketItemsUser1.Should().HaveCount(1);
        }
    }
}
