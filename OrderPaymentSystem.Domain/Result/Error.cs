namespace OrderPaymentSystem.Domain.Result
{
    /// <summary>
    /// Ошибка операции
    /// </summary>
    /// <param name="Message">Сообщение об ошибке</param>
    /// <param name="Code">Статус код ошибки</param>
    public record Error
    {
        public string Message { get; }
        public int Code { get; }

        public Error() { }

        public Error(string message, int code) 
        {
            Message = message;
            Code = code;
        }
    }
}
