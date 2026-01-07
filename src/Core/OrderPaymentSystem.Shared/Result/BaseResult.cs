namespace OrderPaymentSystem.Shared.Result;

/// <summary>
/// Класс для реализации паттерна Result Pattern.
/// Абстракция, которая представляет собой результат выполнения операции
/// </summary>
public class BaseResult
{
    /// <summary>
    /// Конструктор для создания объекта результата внутри класса
    /// </summary>
    /// <param name="error"></param>
    protected BaseResult(Error error = null)
    {
        Error = error ?? new Error("Internal server error", 500);
    }

    /// <summary>
    /// Успешный ли результат
    /// </summary>
    public bool IsSuccess => Error.Message == null;

    /// <summary>
    /// Ошибка
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Создание успешного результата
    /// </summary>
    public static BaseResult Success() => new();

    /// <summary>
    /// Создание ошибочного результата
    /// </summary>
    /// <param name="errorCode">Код ошибки в виде числа</param>
    /// <param name="errorMessage">Сообщение об ошибке</param>
    public static BaseResult Failure(int errorCode, string errorMessage) =>
        new(new Error(errorMessage, errorCode));

    /// <summary>
    /// Создание ошибочного результата
    /// </summary>
    /// <param name="error">Ошибка</param>
    public static BaseResult Failure(Error error) =>
        new(error);
}
