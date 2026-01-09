using OrderPaymentSystem.Domain.Constants;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Роль
/// </summary>
public class Role : IEntityId<int>
{
    /// <summary>
    /// Id роли
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Название роли
    /// </summary>
    public string Name { get; protected set; }

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    protected Role() { }

    /// <summary>
    /// Создать роль
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Созданная роль</returns>
    public static Role Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException(ErrorCodes.RoleNameCannotByEmpty, "Role name cannot be empty.");

        return new Role { Id = default, Name = name };
    }

    /// <summary>
    /// Обновить название роли
    /// </summary>
    /// <param name="newName">Новое название</param>
    /// <returns>Результат обновления</returns>
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new BusinessException(ErrorCodes.RoleNameCannotByEmpty, "Role name cannot be empty.");

        Name = newName;
    }
}
