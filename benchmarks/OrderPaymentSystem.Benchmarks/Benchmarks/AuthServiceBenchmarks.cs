using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.DTOs.Auth;
using OrderPaymentSystem.Application.Interfaces.Auth;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Benchmarks.Base;
using OrderPaymentSystem.DAL.Auth;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Benchmarks.Benchmarks;

[Config(typeof(BenchmarkerConfig))]
[MemoryDiagnoser]
public class AuthServiceBenchmarks : BaseBenchmark
{
    private IAuthService _authService;
    private LoginUserDto _loginDto;
    private const string UserLogin = "bench_user";
    private const string Password = "Password123!";

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserTokenService, UserTokenService>();
        services.AddScoped<IAuthService, AuthService>();
    }

    public override async Task GlobalSetup()
    {
        await base.GlobalSetup();

        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var role = Role.Create(DefaultRoles.User);
        db.Roles.Add(role);

        var user = User.Create(UserLogin, hasher.Hash(Password));
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.UserRoles.Add(UserRole.Create(user.Id, role.Id));
        await db.SaveChangesAsync();

        _authService = ServiceScope.ServiceProvider.GetRequiredService<IAuthService>();
        _loginDto = new LoginUserDto(UserLogin, Password);
    }

    [Benchmark]
    public async Task LoginAsync() => await _authService.LoginAsync(_loginDto);
}