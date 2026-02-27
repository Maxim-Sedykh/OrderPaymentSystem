using NetArchTest.Rules;
using OrderPaymentSystem.ArchitectureTests.Abstract;

namespace OrderPaymentSystem.ArchitectureTests.Tests;

/// <summary>
/// Тестирование богатых доменных моделей.
/// </summary>
public class DomainRulesTests : BaseArchitectureTest
{
    /// <summary>
    /// Все классы в папке Entities в проекте Domain должны иметь только приватные сеттеры.
    /// Чтобы внешний код не мог туда передавать данные, и богатая доменная модель была самодостаточным API
    /// </summary>
    [Fact]
    public void Entities_Should_Have_PrivateSetters()
    {
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Entities")
            .And()
            .AreNotAbstract()
            .GetTypes();

        var failingTypes = new List<string>();

        foreach (var type in entityTypes)
        {
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.GetSetMethod() != null && prop.GetSetMethod()!.IsPublic)
                {
                    failingTypes.Add($"{type.Name}.{prop.Name}");
                }
            }
        }

        Assert.Empty(failingTypes);
    }
}
