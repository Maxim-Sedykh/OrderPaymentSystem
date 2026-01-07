namespace OrderPaymentSystem.Shared.Result;

/// <summary>
/// Ошибка операции
/// </summary>
public record Error
{
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Числовой код ошибки
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Конструктор создания ошибки
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="code">Числовой код создания ошибки</param>
    public Error(string message, int code) 
    {
        Message = message;
        Code = code;
    }
}
