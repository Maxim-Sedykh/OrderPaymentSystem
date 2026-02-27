using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories.Base;

/// <summary>
/// Unit of work. Представляет единую входную точку для всего необходимого взаимодействия с БД.
/// Объекты репозиториев создаются только при первом обращении к нему.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Контекст для работы с БД
    /// </summary>
    private readonly ApplicationDbContext _context;
    
    private IOrderRepository? _orders;
    private IProductRepository? _products;
    private IOrderItemRepository? _orderItems;
    private IBasketItemRepository? _basketItems;
    private IPaymentRepository? _payments;
    private IRoleRepository? _roles;
    private IUserRepository? _users;
    private IUserRoleRepository? _userRoles;
    private IUserTokenRepository? _userToken;
    private readonly Dictionary<Type, Func<object>> _repositoryFactories;

    /// <summary>
    /// Конструктор UnitOfWork
    /// </summary>
    public UnitOfWork(
            ApplicationDbContext context,
            Func<IOrderRepository> orderFactory,
            Func<IProductRepository> productFactory,
            Func<IOrderItemRepository> orderItemFactory,
            Func<IBasketItemRepository> basketItemFactory,
            Func<IPaymentRepository> paymentFactory,
            Func<IRoleRepository> roleFactory,
            Func<IUserRepository> userFactory,
            Func<IUserRoleRepository> userRoleFactory,
            Func<IUserTokenRepository> userTokenFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _repositoryFactories = new Dictionary<Type, Func<object>>
            {
                { typeof(IOrderRepository), orderFactory },
                { typeof(IProductRepository), productFactory },
                { typeof(IOrderItemRepository), orderItemFactory },
                { typeof(IBasketItemRepository), basketItemFactory },
                { typeof(IPaymentRepository), paymentFactory },
                { typeof(IRoleRepository), roleFactory },
                { typeof(IUserRepository), userFactory },
                { typeof(IUserRoleRepository), userRoleFactory },
                { typeof(IUserTokenRepository), userTokenFactory }
            };
    }

    /// <inheritdoc/>
    public IOrderRepository Orders => _orders ??= GetRepository<IOrderRepository>();

    /// <inheritdoc/>
    public IProductRepository Products => _products ??= GetRepository<IProductRepository>();

    /// <inheritdoc/>
    public IOrderItemRepository OrderItems => _orderItems ??= GetRepository<IOrderItemRepository>();

    /// <inheritdoc/>
    public IBasketItemRepository BasketItems => _basketItems ??= GetRepository<IBasketItemRepository>();

    /// <inheritdoc/>
    public IPaymentRepository Payments => _payments ??= GetRepository<IPaymentRepository>();

    /// <inheritdoc/>
    public IRoleRepository Roles => _roles ??= GetRepository<IRoleRepository>();

    /// <inheritdoc/>
    public IUserRepository Users => _users ??= GetRepository<IUserRepository>();

    /// <inheritdoc/>
    public IUserRoleRepository UserRoles => _userRoles ??= GetRepository<IUserRoleRepository>();

    /// <inheritdoc/>
    public IUserTokenRepository UserTokens => _userToken ??= GetRepository<IUserTokenRepository>();

    /// <inheritdoc/>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Получить репозиторий из фабричного словаря
    /// </summary>
    /// <typeparam name="TRepository">Тип репозитория который хотим получить</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">В фабрике не найден тип репозитория</exception>
    private TRepository GetRepository<TRepository>() where TRepository : class
    {
        if (_repositoryFactories.TryGetValue(typeof(TRepository), out var factory))
        {
            return (TRepository)factory();
        }
        throw new InvalidOperationException($"Repository of type {typeof(TRepository).Name} is not registered.");
    }
}
