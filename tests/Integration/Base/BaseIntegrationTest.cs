using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Base;

/// <summary>
/// Базовый класс для интеграционных тестов.
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>
{
    /// <summary>
    /// Http клиент для запросов.
    /// </summary>
    protected readonly HttpClient Client;

    /// <summary>
    /// Фабрика для создания необходимых зависимостей для интеграционного теста.
    /// </summary>
    protected readonly IntegrationTestFactory Factory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="factory">Фабрика.</param>
    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Авторизует клиента под указанной ролью.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="role">Роль пользователя ("User" или "Admin").</param>
    /// <param name="password">Пароль пользователя. Если null, используются дефолтные пароли из TestConstants.</param>
    protected async Task AuthenticateAsync(string login = "admin", string role = "Admin", string password = TestConstants.AdminPassword)
    {
        if (role == DefaultRoles.User)
        {
            var registerDto = new RegisterUserDto(login, password, password);
            await Client.PostAsJsonAsync(TestConstants.ApiAuthRegister, registerDto);
        }

        var loginDto = new LoginUserDto(login, password);
        var response = await Client.PostAsJsonAsync(TestConstants.ApiAuthLogin, loginDto);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token!.AccessToken);
    }

    /// <summary>
    /// Получить товар по Id.
    /// </summary>
    /// <param name="productId">Идентификатор товара.</param>
    /// <returns><see cref="ProductDto"/> или null</returns>
    protected async Task<ProductDto?> GetProductAsync(int productId)
    {
        var response = await Client.GetAsync($"{TestConstants.ApiProductsV1}/{productId}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    /// <summary>
    /// Получить заказ по Id.
    /// </summary>
    /// <param name="orderId">Идентификатор заказа.</param>
    /// <returns><see cref="OrderDto"/> или null</returns>
    protected async Task<OrderDto?> GetOrderAsync(int orderId)
    {
        var response = await Client.GetAsync($"{TestConstants.ApiOrdersV1}/{orderId}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }
}
