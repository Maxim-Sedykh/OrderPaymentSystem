using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового роли
/// </summary>
public class RoleBuilder
{
    private int _id = 1;
    private string _name = "Admin";

    /// <summary>
    /// Добавить Id
    /// </summary>
    public RoleBuilder WithId(int id) { _id = id; return this; }

    /// <summary>
    /// Добавить название
    /// </summary>
    public RoleBuilder WithName(string name) { _name = name; return this; }

    /// <summary>
    /// Построить, создать объект.
    /// </summary>
    /// <returns>Созданная роль</returns>
    public Role Build() => Role.CreateExisting(_id, _name);
}
