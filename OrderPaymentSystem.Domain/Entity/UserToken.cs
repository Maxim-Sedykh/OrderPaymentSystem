using OrderPaymentSystem.Domain.Interfaces;

namespace OrderPaymentSystem.Domain.Entity
{
    public class UserToken : IEntityId<long>
    {
        public long Id { get; set; }

        public string RefreshToken { get; set; } 

        public DateTime RefreshTokenExpireTime { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }
    }
}
