namespace OrderPaymentSystem.Domain.Abstract.Interfaces.Entities
{
    /// <summary>
    /// Определяет сущность, имеющую идентификатор.
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора.</typeparam>
    public interface IEntityId<TId>
    {
        /// <summary>
        /// Уникальный идентификатор сущности.
        /// </summary>
        TId Id { get; }
    }
}
