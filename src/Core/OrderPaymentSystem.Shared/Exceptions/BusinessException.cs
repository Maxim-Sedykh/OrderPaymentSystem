using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Shared.Exceptions;

/// <summary>
/// Базовый класс для бизнес-исключений
/// Используется для обработки ошибок в бизнес-логике доменных моделях
/// </summary>
public class BusinessException : Exception
{
    public int ErrorCode { get; }

    private const string DefaultMessage = "An unexpected business error occurred.";

    public BusinessException(int errorCode, string message = DefaultMessage, Exception innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public BusinessException(Error error, Exception innerException = null)
        : base(error.Message, innerException)
    {
        ErrorCode = error.Code;
    }
}
