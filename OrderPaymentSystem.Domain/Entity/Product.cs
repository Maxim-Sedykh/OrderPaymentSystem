using OrderPaymentSystem.Domain.Interfaces;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Product : IEntityId<int>, IAuditable
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
