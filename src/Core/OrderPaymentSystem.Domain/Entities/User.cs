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
    public string Login { get; private set; } = string.Empty;

    /// <summary>
    /// Пароль в виде хэша
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime CreatedAt { get; private set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Внутренняя коллекция ролей
    /// </summary>
    private readonly List<Role> _roles = [];

    /// <summary>
    /// Роли пользователя. Только для чтения.
    /// </summary>
    public virtual IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Внутренняя коллекция заказов
    /// </summary>
    private readonly List<Order> _orders = [];

    /// <summary>
    /// Заказы пользователя. Только для чтения.
    /// </summary>
    public virtual IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    /// <summary>
    /// Внутренняя коллекция элементов корзины
    /// </summary>
    private readonly List<BasketItem> _basketItems = [];

    /// <summary>
    /// Корзина пользователя. Только для чтения.
    /// </summary>
    public virtual IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();

    /// <summary>
    /// Токен аутентификации
    /// </summary>
    public UserToken? UserToken { get; private set; }

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
    public static User Create(string? login, string? passwordHash)
    {
        Validate(login, passwordHash);

        return new User(login!, passwordHash!);
    }


    /// <summary>
    /// Создать существующего пользователя. Используется только для тестов
    /// </summary>
    internal static User CreateExisting(Guid id, string login, string passwordHash)
    {
        Validate(login, passwordHash);

        return new User(id, login, passwordHash);
    }

    /// <summary>
    /// Поменять пароль пользователя
    /// </summary>
    /// <param name="newPasswordHash">Хэш нового пароля</param>
    /// <returns>Результат замены пароля у пользователя</returns>
    public void ChangePassword(string? newPasswordHash)
    {
        if (PasswordHash == newPasswordHash)
        {
            return;
        }

        Validate(Login, newPasswordHash);

        PasswordHash = newPasswordHash!;
    }

    /// <summary>
    /// Добавить роли пользователю. Только для тестов.
    /// </summary>
    /// <param name="roles"></param>
    internal void AddRoles(params IEnumerable<Role> roles)
    {
        _roles.AddRange(roles);
    }

    /// <summary>
    /// Присвоить токен пользователю. Только для тестов.
    /// </summary>
    /// <param name="token"></param>
    internal void SetToken(UserToken token)
    {
        UserToken = token;
    }

    /// <summary>
    /// Валидировать входные данные
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="password">Пароль</param>
    /// <exception cref="BusinessException"></exception>
    private static void Validate(string? login, string? password)
    {
        if (string.IsNullOrWhiteSpace(login)) throw new BusinessException(DomainErrors.Validation.Required(nameof(login)));
        if (string.IsNullOrWhiteSpace(password)) throw new BusinessException(DomainErrors.Validation.Required(nameof(password)));
    }
}