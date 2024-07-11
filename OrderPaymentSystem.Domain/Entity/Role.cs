using OrderPaymentSystem.Domain.Interfaces;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Role: IEntityId<long>
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
