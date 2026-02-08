using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Пользователь системы
/// </summary>
public class User : BaseEntity<Guid>, IAuditable
{
    /// <summary>
    /// Логин
    /// </summary>
    public string Login { get; private set; }

    /// <summary>
    /// Пароль в виде хэша
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; private set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<Role> _roles = [];
    public virtual IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private readonly List<Order> _orders = [];
    public virtual IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private readonly List<BasketItem> _basketItems = [];
    public virtual IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();

    /// <summary>
    /// Токен аутентификации
    /// </summary>
    public UserToken UserToken { get; private set; }

    private User() { }

    private User(string login, string passwordHash) 
    {
        Id = Guid.NewGuid();
        Login = login;
        PasswordHash = passwordHash;
    }

    private User(Guid id, string login, string passwordHash) 
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Создать пользователя
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="passwordHash">Хэш пароля</param>
    /// <returns>Результат создания пользователя</returns>
    public static User Create(string login, string passwordHash)
    {
        Validate(login, passwordHash);

        return new User(login, passwordHash);
    }

    public static User CreateExisting(Guid id, string login, string passwordHash)
    {
        Validate(login, passwordHash);

        return new User(id, login, passwordHash);
    }

    /// <summary>
    /// Поменять пароль пользователя
    /// </summary>
    /// <param name="newPasswordHash">Хэш нового пароля</param>
    /// <returns>Результат замены пароля у пользователя</returns>
    public void ChangePassword(string newPasswordHash)
    {
        if (PasswordHash == newPasswordHash)
        {
            return;
        }

        Validate(Login, newPasswordHash);

        PasswordHash = newPasswordHash;
    }

    public void AddRole(Role role)
    {
        _roles.Add(role);
    }

    public void SetToken(UserToken token)
    {
        UserToken = token;
    }

    private static void Validate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login)) throw new BusinessException(DomainErrors.Validation.Required(nameof(login)));
        if (string.IsNullOrWhiteSpace(password)) throw new BusinessException(DomainErrors.Validation.Required(nameof(password)));
    }
}