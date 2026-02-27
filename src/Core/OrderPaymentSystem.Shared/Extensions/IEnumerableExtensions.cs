namespace OrderPaymentSystem.Shared.Extensions;

/// <summary>
/// Расширения для IEnumerable
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Null или пустая ли коллекция. Если коллекция null или пустая - то true
    /// </summary>
    /// <typeparam name="T">Тип объектов внутри коллекции</typeparam>
    /// <param name="collection">Коллекция</param>
    /// <returns>bool</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }
}
