using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Domain.Exceptions
{
    /// <summary>
    /// Базовый класс для бизнес-исключений
    /// Используется для обработки ошибок в бизнес-логике доменных моделях
    /// </summary>
    public class BusinessException : Exception
    {
        public int CodeValue { get; }
        public string CodeName { get; }

        public BusinessException(ErrorCodes errorCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            CodeValue = (int)errorCode;
            CodeName = errorCode.ToString();
        }
    }
}
