using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

/// <summary>
/// Билдер для построения мокового пользователя
/// </summary>
public class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _login = "testuser";
    private string _passwordHash = "hashed_password";
    private List<Role> _roles = new();

    /// <summary>
    /// Добавить логин
    /// </summary>
    public UserBuilder WithLogin(string login) { _login = login; return this; }

    /// <summary>
    /// Построить, создать объект.
    /// </summary>
    /// <returns>Созданный пользователь</returns>
    public User Build()
    {
        var user = User.CreateExisting(_id, _login, _passwordHash);
        user.AddRoles(_roles);

        return user;
    }
}
