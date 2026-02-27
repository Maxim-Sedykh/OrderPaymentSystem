using NetArchTest.Rules;
using OrderPaymentSystem.ArchitectureTests.Abstract;

namespace OrderPaymentSystem.ArchitectureTests.Tests;

/// <summary>
/// Тестирование поддержания правил наименований.
/// </summary>
public class NamingDesignTests : BaseArchitectureTest
{
    /// <summary>
    /// Все интерфейсы должны начинаться на букву 'I'
    /// </summary>
    [Fact]
    public void Interfaces_Should_StartWithI()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Все контроллеры должны иметь суффикс 'Controller'
    /// </summary>
    [Fact]
    public void Controllers_Should_Have_ControllerSuffix()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespace($"{PresentationNamespace}.Controllers")
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
