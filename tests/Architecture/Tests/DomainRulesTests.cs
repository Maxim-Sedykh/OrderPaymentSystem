using NetArchTest.Rules;
using OrderPaymentSystem.ArchitectureTests.Abstract;

namespace OrderPaymentSystem.ArchitectureTests.Tests
{
    public class DomainRulesTests : BaseArchitectureTest
    {
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
}
