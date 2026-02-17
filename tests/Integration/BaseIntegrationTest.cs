using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.DTOs.Token;
using OrderPaymentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace OrderPaymentSystem.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>
    {
        protected readonly HttpClient Client;
        protected readonly IntegrationTestFactory Factory;

        protected BaseIntegrationTest(IntegrationTestFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        // Хелпер для авторизации под любым пользователем (Admin или User)
        protected async Task AuthenticateAsync(string login = "admin", string role = "Admin")
        {
            if (role == "User")
            {
                var registerDto = new RegisterUserDto(login, "StrongPass123!", "StrongPass123!");
                await Client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

                // Имитируем логин
                var loginDto = new LoginUserDto(login, "StrongPass123!");
                var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
                var token = await response.Content.ReadFromJsonAsync<TokenDto>();

                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
            }
            if (role == "Admin")
            {
                // Имитируем логин
                var loginDto = new LoginUserDto(login, "12345");
                var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
                var token = await response.Content.ReadFromJsonAsync<TokenDto>();

                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
            }
        }
    }
}
