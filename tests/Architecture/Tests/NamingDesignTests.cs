using NetArchTest.Rules;
using OrderPaymentSystem.ArchitectureTests.Abstract;

namespace OrderPaymentSystem.ArchitectureTests.Tests
{
    public class NamingDesignTests : BaseArchitectureTest
    {
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

        [Fact]
        public void Repositories_Should_ResideInInfrastructure()
        {
            var result = Types.InAssembly(InfrastructureAssembly)
                .That()
                .HaveNameEndingWith("Repository")
                .Should()
                .ResideInNamespace(InfrastructureNamespace)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Services_Should_Be_Internal()
        {
            var result = Types.InAssembly(ApplicationAssembly)
                .That()
                .HaveNameEndingWith("Service")
                .And()
                .AreClasses()
                .Should()
                .NotBePublic()
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }
}
