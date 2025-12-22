using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;

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

    /// <summary>
    /// Роли
    /// </summary>
    public ICollection<Role> Roles { get; protected set; }

    /// <summary>
    /// Заказы
    /// </summary>
    public ICollection<Order> Orders { get; protected set; }

    /// <summary>
    /// Элементы корзины
    /// </summary>
    public ICollection<BasketItem> BasketItems { get; protected set; }

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
    public static DataResult<User> Create(string login, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(login)) return DataResult<User>.Failure(4001, "Login cannot be empty.");
        if (string.IsNullOrWhiteSpace(passwordHash)) return DataResult<User>.Failure(4002, "Password hash cannot be empty.");

        return DataResult<User>.Success(new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            PasswordHash = passwordHash
        });
    }

    /// <summary>
    /// Поменять пароль пользователя
    /// </summary>
    /// <param name="newPasswordHash">Хэш нового пароля</param>
    /// <returns>Результат замены пароля у пользователя</returns>
    public BaseResult ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash)) return BaseResult.Failure(4003, "New password hash cannot be empty.");
        PasswordHash = newPasswordHash;

        return BaseResult.Success();
    }
}