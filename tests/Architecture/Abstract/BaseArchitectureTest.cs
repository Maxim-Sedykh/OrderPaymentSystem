using OrderPaymentSystem.Api.Controllers;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Result;
using System.Reflection;

namespace OrderPaymentSystem.ArchitectureTests.Abstract;

/// <summary>
/// Базовый абстрактный класс для архитектурных тестов.
/// </summary>
public abstract class BaseArchitectureTest
{
    /// <summary>
    /// Сборка проекта Domain.
    /// </summary>
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;

    /// <summary>
    /// Сборка проекта Application.
    /// </summary>
    protected static readonly Assembly ApplicationAssembly = typeof(IAuthService).Assembly;

    /// <summary>
    /// Сборка проекта DAL.
    /// </summary>
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;

    /// <summary>
    /// Сборка проекта WebApi.
    /// </summary>
    protected static readonly Assembly PresentationAssembly = typeof(AuthController).Assembly;

    /// <summary>
    /// Сборка проекта Shared.
    /// </summary>
    protected static readonly Assembly SharedAssembly = typeof(Error).Assembly;

    /// <summary>
    /// Название пространства имён в Domain-проекте.
    /// </summary>
    protected const string DomainNamespace = "OrderPaymentSystem.Domain";

    /// <summary>
    /// Название пространства имён в Application-проекте.
    /// </summary>
    protected const string ApplicationNamespace = "OrderPaymentSystem.Application";

    /// <summary>
    /// Название пространства имён в DAL-проекте.
    /// </summary>
    protected const string InfrastructureNamespace = "OrderPaymentSystem.DAL";

    /// <summary>
    /// Название пространства имён в Api-проекте.
    /// </summary>
    protected const string PresentationNamespace = "OrderPaymentSystem.Api";

    /// <summary>
    /// Название пространства имён в Shared-проекте.
    /// </summary>
    protected const string SharedNamespace = "OrderPaymentSystem.Shared";
}