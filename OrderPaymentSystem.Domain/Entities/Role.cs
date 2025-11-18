using OrderPaymentSystem.Domain.Interfaces.Entities;

namespace OrderPaymentSystem.Domain.Entities;

public class Role: IEntityId<long>
{
    public long Id { get; set; }

    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
}
