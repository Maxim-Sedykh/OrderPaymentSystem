using OrderPaymentSystem.Domain.Abstract;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Shared.Exceptions;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Роль
/// </summary>
public class Role : BaseEntity<int>
{
    /// <summary>
    /// Название роли
    /// </summary>
    public string Name { get; private set; }

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Role() { }

    private Role(string name)
    {
        Name = name;
    }

    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    internal static Role CreateExisting(int id, string name)
    {
        if (id < 0)
            throw new BusinessException(DomainErrors.Validation.InvalidFormat(nameof(id)));

        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException(DomainErrors.Validation.Required(nameof(Name)));

        return new Role(id, name);
    }

    /// <summary>
    /// Создать роль
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Созданная роль</returns>
    public static Role Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException(DomainErrors.Validation.Required(nameof(Name)));

        return new Role(name);
    }

    /// <summary>
    /// Обновить название роли
    /// </summary>
    /// <param name="newName">Новое название</param>
    /// <returns>Результат обновления</returns>
    public void UpdateName(string newName)
    {
        if (newName == Name)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(newName))
            throw new BusinessException(DomainErrors.Validation.Required(nameof(Name)));

        Name = newName;
    }
}
