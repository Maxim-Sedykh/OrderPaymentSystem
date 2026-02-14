using OrderPaymentSystem.Api.Controllers;
using OrderPaymentSystem.Application.Interfaces.Services;
using OrderPaymentSystem.DAL.Persistence;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Shared.Result;
using System.Reflection;

namespace OrderPaymentSystem.ArchitectureTests.Abstract;

public abstract class BaseArchitectureTest
{
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IAuthService).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(AuthController).Assembly;
    protected static readonly Assembly SharedAssembly = typeof(Error).Assembly;

    protected const string DomainNamespace = "OrderPaymentSystem.Domain";
    protected const string ApplicationNamespace = "OrderPaymentSystem.Application";
    protected const string InfrastructureNamespace = "OrderPaymentSystem.DAL";
    protected const string PresentationNamespace = "OrderPaymentSystem.Api";
    protected const string SharedNamespace = "OrderPaymentSystem.Shared";
}