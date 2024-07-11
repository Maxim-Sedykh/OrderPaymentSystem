using OrderPaymentSystem.Domain.Interfaces;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Basket: IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public ICollection<Order> Orders { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
