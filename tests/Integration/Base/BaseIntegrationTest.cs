using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.Product;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.IntegrationTests.Constants;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderPaymentSystem.IntegrationTests.Base;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>
{
    protected readonly HttpClient Client;
    protected readonly IntegrationTestFactory Factory;

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
    protected async Task AuthenticateAsync(string login = "admin", string role = "Admin", string password = null)
    {
        string userPassword = password ?? (role == DefaultRoles.User ? TestConstants.CommonUserPassword : TestConstants.AdminPassword);

        if (role == TestConstants.UserRole)
        {
            var registerDto = new RegisterUserDto(login, userPassword, userPassword);
            await Client.PostAsJsonAsync(TestConstants.ApiAuthRegister, registerDto);
        }

        var loginDto = new LoginUserDto(login, userPassword);
        var response = await Client.PostAsJsonAsync(TestConstants.ApiAuthLogin, loginDto);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
    }

    protected async Task<ProductDto> GetProductAsync(int productId)
    {
        var response = await Client.GetAsync($"{TestConstants.ApiProductsV1}/{productId}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    protected async Task<OrderDto> GetOrderAsync(int orderId)
    {
        var response = await Client.GetAsync($"{TestConstants.ApiOrdersV1}/{orderId}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }
}
