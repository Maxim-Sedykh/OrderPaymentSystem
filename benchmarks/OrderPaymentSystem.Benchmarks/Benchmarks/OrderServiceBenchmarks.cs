using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Application.DTOs.OrderItem;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.Application.Services.Orders;
using OrderPaymentSystem.Benchmarks.Base;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.ValueObjects;

namespace OrderPaymentSystem.Benchmarks.Benchmarks;

/// <summary>
/// Бенчмарк <see cref="OrderService"/>.
/// Замер производительности.
/// </summary>
[Config(typeof(BenchmarkerConfig))]
[MemoryDiagnoser]
public class OrderServiceBenchmarks : BaseBenchmark
{
    /// <summary>
    /// DTO для создания заказа с маленьким количеством элементов
    /// </summary>
    private CreateOrderDto? _smallOrderDto;

    /// <summary>
    /// DTO для создания заказа с большим количеством элементов
    /// </summary>
    private CreateOrderDto? _largeOrderDto;

    /// <summary>
    /// Id мокового пользователя
    /// </summary>
    private Guid _testUserId;

    /// <summary>
    /// Идентификаторы товаров для маленького заказа
    /// </summary>
    private List<int> _productIdsSmall = [];

    /// <summary>
    /// Идентификаторы товара для большого заказа
    /// </summary>
    private List<int> _productIdsLarge = [];

    /// <inheritdoc/>
    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
    }

    /// <inheritdoc/>
    public override async Task GlobalSetup()
    {
        await base.GlobalSetup();

        using var scope = ServiceProvider!.CreateScope();
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

        _productIdsSmall = [.. smallProducts.Select(p => p.Id)];
        _productIdsLarge = [.. largeProducts.Select(p => p.Id)];

        var user = User.Create("order_user", "hashed_password");
        _testUserId = user.Id;
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        _smallOrderDto = new CreateOrderDto
        {
            DeliveryAddress = new Address("test", "test", "test", "test"),
            OrderItems = _productIdsSmall.Take(5).Select(id => new CreateOrderItemDto { ProductId = id, Quantity = 1 }).ToList()
        };

        _largeOrderDto = new CreateOrderDto
        {
            DeliveryAddress = new Address("test", "test", "test", "test"),
            OrderItems = _productIdsLarge.Select(id => new CreateOrderItemDto { ProductId = id, Quantity = 1 }).ToList()
        };
    }

    /// <summary>
    /// Измерить скорость и производительность метода CreateAsync в OrderService.
    /// С маленьким количеством элементов заказа.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Order_Create")]
    public async Task CreateOrder_Small()
    {
        using var scope = ServiceProvider!.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

        _smallOrderDto!.DeliveryAddress = new Address("CountryS", "CityS", "StreetS", "12345");

        var result = await orderService.CreateAsync(_testUserId, _smallOrderDto);

        if (!result.IsSuccess) throw new Exception(result.Error!.Message);
    }

    /// <summary>
    /// Измерить скорость и производительность метода CreateAsync в OrderService.
    /// С большим количеством элементов заказа.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Order_Create")]
    public async Task CreateOrder_Large()
    {
        using var scope = ServiceProvider!.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

        _largeOrderDto!.DeliveryAddress = new Address("CountryL", "CityL", "StreetL", "67890");

        var result = await orderService.CreateAsync(_testUserId, _largeOrderDto);

        if (!result.IsSuccess) throw new Exception(result.Error!.Message);
    }
}
