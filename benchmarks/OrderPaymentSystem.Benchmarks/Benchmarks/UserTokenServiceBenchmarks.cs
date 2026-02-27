using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Services.Auth;
using OrderPaymentSystem.Benchmarks.Base;
using System.Security.Claims;

namespace OrderPaymentSystem.Benchmarks.Benchmarks;

/// <summary>
/// Бенчмарк <see cref="UserTokenService"/>.
/// Замер производительности.
/// </summary>
[Config(typeof(BenchmarkerConfig))]
[MemoryDiagnoser]
public class UserTokenServiceBenchmarks : BaseBenchmark
{
    /// <summary>
    /// Сервис для работы с токенами
    /// </summary>
    private UserTokenService? _tokenService;

    /// <summary>
    /// Лист клеймов, по которому будет создаваться Access-токен
    /// </summary>
    private List<Claim> _claims = [];

    /// <inheritdoc/>
    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<UserTokenService>();
    }

    /// <inheritdoc/>
    public override async Task GlobalSetup()
    {
        await base.GlobalSetup();
        _tokenService = ServiceScope!.ServiceProvider.GetRequiredService<UserTokenService>();
        _claims =
        [
            new Claim(ClaimTypes.Name, "user"),
            new Claim(ClaimTypes.Role, "Admin")
        ];
    }

    /// <summary>
    /// Измерить скорость и производительность метода GenerateAccessToken в <see cref="UserTokenService"/>.
    /// Без кэша
    /// </summary>
    [Benchmark]
    public string GenerateToken() => _tokenService!.GenerateAccessToken(_claims);
}
