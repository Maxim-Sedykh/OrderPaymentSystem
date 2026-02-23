using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Benchmarks.Base;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.Benchmarks.Benchmarks;

[Config(typeof(BenchmarkerConfig))]
[MemoryDiagnoser]
public class OrderServiceBenchmarks : BaseBenchmark
{
    private CreateOrderDto _smallOrderDto;
    private CreateOrderDto _largeOrderDto;
    private Guid _testUserId;
    private List<int> _productIdsSmall;
    private List<int> _productIdsLarge;

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
    }

    public override async Task GlobalSetup()
    {
        await base.GlobalSetup();
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var smallProducts = Enumerable.Range(1, 100)
            .Select(i => Product.Create($"Small {i}", "Desc", 100m + i, 100))
            .ToList();

        var largeProducts = Enumerable.Range(1, 1000)
            .Select(i => Product.Create($"Large {i}", "Desc", 100m + i, 100))
            .ToList();

        dbContext.Products.AddRange(smallProducts);
        dbContext.Products.AddRange(largeProducts);
        await dbContext.SaveChangesAsync();

        _productIdsSmall = smallProducts.Select(p => p.Id).ToList();
        _productIdsLarge = largeProducts.Select(p => p.Id).ToList();

        var user = User.Create("order_user", "hashed_password");
        _testUserId = user.Id;
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        _smallOrderDto = new CreateOrderDto
        {
            DeliveryAddress = new Address("CountryS", "CityS", "StreetS", "12345"),
            OrderItems = _productIdsSmall.Take(5).Select(id => new CreateOrderItemDto { ProductId = id, Quantity = 1 }).ToList()
        };

        _largeOrderDto = new CreateOrderDto
        {
            DeliveryAddress = new Address("CountryL", "CityL", "StreetL", "67890"),
            OrderItems = _productIdsLarge.Select(id => new CreateOrderItemDto { ProductId = id, Quantity = 1 }).ToList()
        };
    }

    [Benchmark]
    [BenchmarkCategory("Order_Create")]
    public async Task CreateOrder_Small()
    {
        using var scope = ServiceProvider.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

        _smallOrderDto.DeliveryAddress = new Address("CountryS", "CityS", "StreetS", "12345");

        var result = await orderService.CreateAsync(_testUserId, _smallOrderDto);

        if (!result.IsSuccess) throw new Exception(result.Error.Message);
    }

    [Benchmark]
    [BenchmarkCategory("Order_Create")]
    public async Task CreateOrder_Large()
    {
        using var scope = ServiceProvider.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

        _largeOrderDto.DeliveryAddress = new Address("CountryL", "CityL", "StreetL", "67890");

        var result = await orderService.CreateAsync(_testUserId, _largeOrderDto);

        if (!result.IsSuccess) throw new Exception(result.Error.Message);
    }
}
