using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services.Auth;
using OrderPaymentSystem.Benchmarks.Base;
using System.Security.Claims;

namespace OrderPaymentSystem.Benchmarks.Benchmarks;

[Config(typeof(BenchmarkerConfig))]
[MemoryDiagnoser]
public class UserTokenServiceBenchmarks : BaseBenchmark
{
    private IUserTokenService _tokenService;
    private List<Claim> _claims;

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IUserTokenService, UserTokenService>();
    }

    public override async Task GlobalSetup()
    {
        await base.GlobalSetup();
        _tokenService = ServiceScope.ServiceProvider.GetRequiredService<IUserTokenService>();
        _claims =
        [
            new Claim(ClaimTypes.Name, "user"),
            new Claim(ClaimTypes.Role, "Admin")
        ];
    }

    [Benchmark]
    public string GenerateToken() => _tokenService.GenerateAccessToken(_claims);
}
