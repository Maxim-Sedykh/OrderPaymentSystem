namespace OrderPaymentSystem.Shared.Result;

/// <summary>
/// Класс для реализации паттерна Result Pattern.
/// Абстракция, которая представляет собой результат выполнения операции
/// Имеет свойство Data
/// </summary>
/// <typeparam name="T">Тип данных в результате</typeparam>
public class DataResult<T> : BaseResult
{
    /// <summary>
    /// Конструктор создания результата с данными
    /// </summary>
    /// <param name="data">Данные</param>
    /// <param name="error">Ошибка, необязательный параметр</param>
    protected DataResult(T data, Error error = null)
        : base(error)
    {
        Data = data;
    }

    /// <summary>
    /// Данные
    /// </summary>
    public T Data { get; }

    /// <summary>
    /// Создать успешный результат с данными
    /// </summary>
    /// <param name="data">Данные</param>
    public static DataResult<T> Success(T data) =>
        new(data);

    public static new DataResult<T> Failure(int errorCode, string errorMessage) =>
        new(default, new Error(errorMessage, errorCode));

    public static new DataResult<T> Failure(Error error) => new(default, error);
}
