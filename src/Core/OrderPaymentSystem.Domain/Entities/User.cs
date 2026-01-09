using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Пользователь системы
/// </summary>
public class User : IEntityId<Guid>, IAuditable
{
    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Логин
    /// </summary>
    public string Login { get; protected set; }

    /// <summary>
    /// Пароль в виде хэша
    /// </summary>
    public string PasswordHash { get; protected set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; protected set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; protected set; }

    private readonly List<Role> _roles = new();
    public virtual IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private readonly List<Order> _orders = new();
    public virtual IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private readonly List<BasketItem> _basketItems = new();
    public virtual IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();

    /// <summary>
    /// Токен аутентификации
    /// </summary>
    public UserToken UserToken { get; protected set; }

    protected User() { }

    /// <summary>
    /// Создать пользователя
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="passwordHash">Хэш пароля</param>
    /// <returns>Результат создания пользователя</returns>
    public static User Create(string login, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(login)) 
            throw new BusinessException(4001, "Login cannot be empty.");

        if (string.IsNullOrWhiteSpace(passwordHash)) 
            throw new BusinessException(4002, "Password hash cannot be empty.");

        return new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            PasswordHash = passwordHash
        };
    }

    /// <summary>
    /// Поменять пароль пользователя
    /// </summary>
    /// <param name="newPasswordHash">Хэш нового пароля</param>
    /// <returns>Результат замены пароля у пользователя</returns>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash)) 
            throw new BusinessException(4003, "New password hash cannot be empty.");

        PasswordHash = newPasswordHash;
    }
}