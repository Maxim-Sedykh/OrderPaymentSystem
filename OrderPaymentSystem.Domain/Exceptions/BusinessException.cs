namespace OrderPaymentSystem.Domain.Exceptions
{
    /// <summary>
    /// Базовый класс для бизнес-исключений
    /// Используется для обработки ошибок в бизнес-логике доменных моделях
    /// </summary>
    public class BusinessException : Exception
    {
        public int Code { get; }

        public BusinessException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
