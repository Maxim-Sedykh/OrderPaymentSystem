using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;

public class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _login = "testuser";
    private string _passwordHash = "hashed_password";
    private List<Role> _roles = new();

    public UserBuilder WithId(Guid id) { _id = id; return this; }
    public UserBuilder WithLogin(string login) { _login = login; return this; }
    public UserBuilder WithRoles(params Role[] roles) { _roles.AddRange(roles); return this; }

    public User Build()
    {
        var user = User.CreateExisting(_id, _login, _passwordHash);
        user.AddRoles(_roles);

        return user;
    }
}
