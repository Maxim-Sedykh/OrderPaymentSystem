namespace OrderPaymentSystem.Domain.Interfaces.Entities;

public interface IEntityId<T> where T : struct
{
    public T Id { get; set; }
}
