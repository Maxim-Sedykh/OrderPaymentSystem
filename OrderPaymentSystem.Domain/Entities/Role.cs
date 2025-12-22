using OrderPaymentSystem.Domain.Interfaces.Entities;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Entities;

/// <summary>
/// Роль
/// </summary>
public class Role : IEntityId<long>
{
    /// <summary>
    /// Id роли
    /// </summary>
    public long Id { get; protected set; }

    /// <summary>
    /// Название роли
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Пользователи у которых есть эта роль
    /// </summary>
    public ICollection<User> Users { get; protected set; }

    protected Role() { }

    /// <summary>
    /// Создать роль
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Результат создания</returns>
    public static DataResult<Role> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return DataResult<Role>.Failure(6001, "Role name cannot be empty.");

        return DataResult<Role>.Success(new Role { Id = default, Name = name });
    }

    /// <summary>
    /// Обновить название роли
    /// </summary>
    /// <param name="newName">Новое название</param>
    /// <returns>Результат обновления</returns>
    public BaseResult UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return BaseResult.Failure(6002, "Role name cannot be empty.");
        Name = newName;

        return BaseResult.Success();
    }
}
