using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;

namespace OrderPaymentSystem.Domain.Abstract;

/// <summary>
/// Абстрактный базовый класс для всех сущностей предметной области, имеющих идентификатор.
/// </summary>
/// <typeparam name="TId">Тип идентификатора.</typeparam>
public abstract class BaseEntity<TId> : IEntityId<TId?>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    public TId? Id { get; protected set; }

    /// <summary>
    /// Защищенный конструктор по умолчанию.
    /// Используется ORM для создания экземпляров сущностей или дочерними классами
    /// при создании новых объектов (где Id еще не определен).
    /// </summary>
    protected BaseEntity() { }

    /// <summary>
    /// Защищенный конструктор для создания сущности с уже известным идентификатором.
    /// Используется для регидрации сущностей из хранилища (ORM) или для тестов.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    protected BaseEntity(TId id)
    {
        if (id.Equals(default))
        {
            throw new ArgumentException($"The provided Id cannot be the default value for type {typeof(TId).Name}.", nameof(id));
        }

        Id = id;
    }

    /// <summary>
    /// Определяет, равен ли текущий объект другому объекту.
    /// Сущности считаются равными, если они имеют одинаковый тип и одинаковый идентификатор.
    /// </summary>
    /// <param name="obj">Объект для сравнения с текущим объектом.</param>
    /// <returns>true, если указанный объект равен текущему объекту; в противном случае, false.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        if (obj is null || GetType() != obj.GetType()) return false;

        var other = (BaseEntity<TId>)obj;
        if (Id is not null && (Id.Equals(default(TId)) || other.Id!.Equals(default)))
        {
            return false;
        }

        return Id!.Equals(other.Id);
    }

    /// <summary>
    /// Возвращает хэш-код для данного экземпляра.
    /// Хэш-код основан на идентификаторе сущности.
    /// </summary>
    /// <returns>Хэш-код для данного экземпляра.</returns>
    public override int GetHashCode()
    {
        if (Id is not null && Id.Equals(default(TId)))
        {
            return 0;
        }
        return Id!.GetHashCode();
    }

    /// <summary>
    /// Переопределение оператора == для идентификатора сущности
    /// </summary>
    /// <param name="left">Левый Id</param>
    /// <param name="right">Правый Id</param>
    /// <returns>True если равны</returns>
    public static bool operator ==(BaseEntity<TId> left, BaseEntity<TId> right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Переопределение оператора != для идентификатора сущности
    /// </summary>
    /// <param name="left">Левый Id</param>
    /// <param name="right">Правый Id</param>
    /// <returns>True если не равны</returns>
    public static bool operator !=(BaseEntity<TId> left, BaseEntity<TId> right)
    {
        return !(left == right);
    }
}
