namespace OrderPaymentSystem.Domain.Result
{
    /// <summary>
    /// Класс для реализации паттерна Result Pattern.
    /// Абстракция, которая представляет собой результат выполнения операции
    /// </summary>
    public class BaseResult
    {
        protected BaseResult(Error error = null)
        {
            Error = error ?? new Error();
        }

        public bool IsSuccess => Error.Message == null;

        public Error Error { get; }

        public static BaseResult Success() => new();

        public static BaseResult Failure(int errorCode, string errorMessage) =>
            new(new Error(errorMessage, errorCode));

        public static BaseResult Failure(Error error) =>
            new(error);
    }
}
