using OrderPaymentSystem.Domain.Interfaces;

namespace OrderPaymentSystem.Domain.Entity
{
    public class User : IEntityId<Guid>, IAuditable
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public ICollection<Role> Roles { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public ICollection<Order> Orders { get; set; }

        public Basket Basket { get; set; }

        public UserToken UserToken { get; set; }    
        
    }
}
