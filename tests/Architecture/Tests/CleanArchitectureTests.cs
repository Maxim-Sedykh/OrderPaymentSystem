using NetArchTest.Rules;
using OrderPaymentSystem.ArchitectureTests.Abstract;

namespace OrderPaymentSystem.ArchitectureTests.Tests;

/// <summary>
/// Тестирование поддержания Clean-архитектуры в проекте.
/// </summary>
public class ArchitectureTests : BaseArchitectureTest
{
    /// <summary>
    /// Domain не должен ни от кого зависеть, все должны зависеть от Domain.
    /// </summary>
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, PresentationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Слой бизнес логики (Application) не должен знать детали реализации БД из слоя DAL.
    /// Проверяет что Application не имеет ссылку на проект DAL.
    /// </summary>
    [Fact]
    public void Application_Should_Not_HaveDependencyOnInfrastructureOrPresentation()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, PresentationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Infrastructure слой не должен быть зависим от Presentation.
    /// </summary>
    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnPresentation()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(PresentationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Shared проект не должен быть ниоткого зависим.
    /// Потому что это просто библиотека с Shared кодом, чтобы её использовали другие проекты.
    /// </summary>
    [Fact]
    public void Shared_Should_Not_HaveDependencyOnOtherProjects()
    {
        var result = Types.InAssembly(SharedAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, PresentationNamespace, DomainNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Проверяет, что все реализации репозиториев, оканчивающиеся на "Repository"
    /// должны находиться исключительно в проекте Infrastructure.
    /// Это гарантирует, что детали реализации доступа к данным изолированы в соответствующем слое
    /// и не протекают в другие части приложения.
    /// </summary>
    [Fact]
    public void RepositoryImplementations_Should_ResideInInfrastructure()
    {
        var result = Types.InAssemblies([DomainAssembly, ApplicationAssembly, InfrastructureAssembly, PresentationAssembly, SharedAssembly])
            .That()
            .AreClasses()
            .And()
            .HaveNameEndingWith("Repository")
            .Should()
            .ResideInNamespace(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Реализации интерфейсов должны быть доступны только в своём проекте,
    /// так как пользователям сервисов не нужно знать их реализацию.
    /// </summary>
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
