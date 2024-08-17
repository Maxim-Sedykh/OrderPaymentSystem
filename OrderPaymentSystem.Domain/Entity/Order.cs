using OrderPaymentSystem.Domain.Interfaces.Entities;

namespace OrderPaymentSystem.Domain.Entity
{
    public class Order : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public long? BasketId { get; set; }

        public virtual Basket Basket { get; set; }

        public long? PaymentId { get; set; }

        public virtual Payment Payment { get; set; }

        public decimal OrderCost { get; set; }

        public int ProductCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
