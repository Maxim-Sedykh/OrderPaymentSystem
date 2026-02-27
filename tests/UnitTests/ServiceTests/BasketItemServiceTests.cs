using FluentAssertions;
using OrderPaymentSystem.Application.DTOs;
using OrderPaymentSystem.Application.DTOs.Basket;
using OrderPaymentSystem.Application.Services.BasketItems;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.Resources;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.UnitTests.Configurations.Factories;
using OrderPaymentSystem.UnitTests.Configurations.Fixtures;

namespace OrderPaymentSystem.UnitTests.ServiceTests;

/// <summary>
/// Тесты сервиса <see cref="BasketItemService"/>
/// </summary>
public class BasketItemServiceTests
{
    private readonly BasketItemFixture _fixture;

    /// <summary>
    /// Конструктор. Инициализация фикстуры
    /// </summary>
    public BasketItemServiceTests()
    {
        _fixture = new BasketItemFixture();
    }

    /// <summary>
    /// Создание элемента, когда товар в элементе найден - должно быть успешным
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenProductExists_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var product = TestDataFactory.Product.Build();
        var dto = new CreateBasketItemDto(product.Id, 5);
        var basketItem = TestDataFactory.CreateBasketItemDto(userId: userId, productId: product.Id, quantity: dto.Quantity);

        _fixture.SetupProduct(product)
                .SetupMapping<BasketItem, BasketItemDto>(basketItem);

        // Act
        var result = await _fixture.Service.CreateAsync(userId, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Quantity.Should().Be(dto.Quantity);
        _fixture.VerifyBasketItemCreated();
        _fixture.VerifySaved();
    }

    /// <summary>
    /// Создание элемента, когда товар в элементе не найден - должно быть в ошибкой
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenProductNotFound_ShouldReturnError()
    {
        // Arrange
        _fixture.SetupProduct(null);

        // Act
        var result = await _fixture.Service.CreateAsync(Guid.NewGuid(), new CreateBasketItemDto(99, 1));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Product.NotFound(99));
        _fixture.VerifyNotSaved();
    }

    /// <summary>
    /// Удаление элемента по Id когда он существует должно быть успешным
    /// </summary>
    [Fact]
    public async Task DeleteByIdAsync_WhenItemExists_ShouldRemoveItem()
    {
        // Arrange
        var item = TestDataFactory.BasketItem.Build();
        _fixture.SetupBasketItem(item);

        // Act
        var result = await _fixture.Service.DeleteByIdAsync(item.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fixture.VerifyBasketItemRemoved(item);
        _fixture.VerifySaved();
    }

    /// <summary>
    /// Обновление количества запрошенного товара с новым валидным количеством товара должно быть успешным
    /// </summary>
    [Fact]
    public async Task UpdateQuantityAsync_WhenDataIsValid_ShouldUpdateQuantity()
    {
        // Arrange
        const int newQty = 10;
        var item = TestDataFactory.BasketItem.WithQuantity(5).Build();

        _fixture.SetupBasketItem(item)
                .SetupMapping<BasketItem, BasketItemDto>(TestDataFactory.CreateBasketItemDto());

        // Act
        var result = await _fixture.Service.UpdateQuantityAsync(item.Id, new UpdateQuantityDto { NewQuantity = newQty });

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(newQty);
        _fixture.VerifySaved();
    }

    /// <summary>
    /// Обновление количества запрошенного товара с новым инвалидным количеством товара должно быть с ошибкой
    /// </summary>
    [Fact]
    public async Task UpdateQuantityAsync_WhenQuantityIsInvalid_ShouldThrowBusinessException()
    {
        // Arrange
        var item = TestDataFactory.BasketItem.Build();
        _fixture.SetupBasketItem(item);
        var updateDto = new UpdateQuantityDto { NewQuantity = 0 };

        // Act
        Func<Task> act = () => _fixture.Service.UpdateQuantityAsync(item.Id, updateDto);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
                  .WithMessage(ErrorMessage.QuantityPositive);
        _fixture.VerifyNotSaved();
    }
}
