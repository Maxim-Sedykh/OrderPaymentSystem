using Microsoft.EntityFrameworkCore.Storage;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;

namespace OrderPaymentSystem.DAL.Persistence.Repositories.Base;

/// <summary>
/// Unit of work. Сервис для работы с транзакциями EF Core
/// </summary>
/// <param name="context"></param>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IOrderRepository _orders;
    private IProductRepository _products;
    private IOrderItemRepository _orderItems;
    private IBasketItemRepository _basketItems;
    private IPaymentRepository _payments;
    private IRoleRepository _roles;
    private IUserRepository _users;
    private IUserRoleRepository _userRoles;
    private IUserTokenRepository _userToken;

    private readonly IDictionary<Type, Func<object>> _repositoryFactories;

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

    public IOrderRepository Orders => _orders ??= GetRepository<IOrderRepository>();
    public IProductRepository Products => _products ??= GetRepository<IProductRepository>();
    public IOrderItemRepository OrderItems => _orderItems ??= GetRepository<IOrderItemRepository>();
    public IBasketItemRepository BasketItems => _basketItems ??= GetRepository<IBasketItemRepository>();
    public IPaymentRepository Payments => _payments ??= GetRepository<IPaymentRepository>();
    public IRoleRepository Roles => _roles ??= GetRepository<IRoleRepository>();
    public IUserRepository Users => _users ??= GetRepository<IUserRepository>();
    public IUserRoleRepository UserRoles => _userRoles ??= GetRepository<IUserRoleRepository>();
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
    /// Универсальный метод для получения репозитория из фабрики
    /// </summary>
    private TRepository GetRepository<TRepository>() where TRepository : class
    {
        if (_repositoryFactories.TryGetValue(typeof(TRepository), out var factory))
        {
            return (TRepository)factory();
        }
        throw new InvalidOperationException($"Repository of type {typeof(TRepository).Name} is not registered.");
    }
}
