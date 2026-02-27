using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Shared.Exceptions;

/// <summary>
/// Базовый класс для бизнес-исключений
/// Используется для обработки ошибок в бизнес-логике доменных моделях
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public int ErrorCode { get; }

    /// <summary>
    /// Сообщение об ошибке по умолчанию
    /// </summary>
    private const string DefaultMessage = "An unexpected business error occurred.";

    /// <summary>
    /// Создать исключение
    /// </summary>
    /// <param name="errorCode">Статус ошибки</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение, необязательный параметр</param>
    public BusinessException(int errorCode, string message = DefaultMessage, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Создать исключение по ошибке
    /// </summary>
    /// <param name="error">Ошибка</param>
    /// <param name="innerException">Внутреннее исключение</param>
    public BusinessException(Error error, Exception? innerException = null)
        : base(error.Message, innerException)
    {
        ErrorCode = error.Code;
    }
}
