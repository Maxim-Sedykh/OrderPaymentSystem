using NetArchTest.Rules;
using OrderPaymentSystem.ArchitectureTests.Abstract;

namespace OrderPaymentSystem.ArchitectureTests.Tests
{
    public class ArchitectureTests : BaseArchitectureTest
    {
        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProjects()
        {
            var result = Types.InAssembly(DomainAssembly)
                .ShouldNot()
                .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, PresentationNamespace)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Application_Should_Not_HaveDependencyOnInfrastructureOrPresentation()
        {
            var result = Types.InAssembly(ApplicationAssembly)
                .ShouldNot()
                .HaveDependencyOnAny(InfrastructureNamespace, PresentationNamespace)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Infrastructure_Should_Not_HaveDependencyOnPresentation()
        {
            var result = Types.InAssembly(InfrastructureAssembly)
                .ShouldNot()
                .HaveDependencyOn(PresentationNamespace)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Shared_Should_Not_HaveDependencyOnOtherProjects()
        {
            var result = Types.InAssembly(SharedAssembly)
                .ShouldNot()
                .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, PresentationNamespace, DomainNamespace)
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }
}
