using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.UnitTests.Configurations.Factories.Builders;


public class RoleBuilder
{
    private int _id = 1;
    private string _name = "Admin";

    public RoleBuilder WithId(int id) { _id = id; return this; }
    public RoleBuilder WithName(string name) { _name = name; return this; }

    public Role Build() => Role.CreateExisting(_id, _name);
}
