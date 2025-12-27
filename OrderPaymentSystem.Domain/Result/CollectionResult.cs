using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Domain.Result
{
    /// <summary>
    /// Реализация паттерна Result Pattern для работы с коллекциями данных.
    /// Представляет результат операции, содержащий коллекцию элементов или ошибку.
    /// </summary>
    /// <typeparam name="T">Тип элементов в коллекции</typeparam>
    public class CollectionResult<T> : DataResult<IReadOnlyCollection<T>>
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CollectionResult{T}"/>
        /// </summary>
        /// <param name="data">Коллекция данных</param>
        /// <param name="error">Ошибка выполнения операции</param>
        private CollectionResult(IReadOnlyCollection<T> data, Error error = null)
            : base(data, error)
        {
            Count = data?.Count ?? 0;
        }

        /// <summary>
        /// Получает количество элементов в коллекции.
        /// Возвращает 0, если операция завершилась ошибкой или коллекция пуста.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Создает успешный результат операции с массивом данных.
        /// </summary>
        /// <param name="data">Массив данных</param>
        /// <returns>Успешный результат с коллекцией данных</returns>
        public static new CollectionResult<T> Success(IReadOnlyCollection<T> data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            return new CollectionResult<T>(data);
        }

        /// <summary>
        /// Создает неуспешный результат операции с указанной ошибкой.
        /// </summary>
        /// <param name="errorCode">Код ошибки</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <returns>Неуспешный результат с ошибкой</returns>
        /// <exception cref="ArgumentNullException">Возникает, если errorMessage является null или пустым</exception>
        public static new CollectionResult<T> Failure(int errorCode, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be null or empty", nameof(errorMessage));

            return new CollectionResult<T>(
                Array.Empty<T>(),
                new Error(errorMessage, errorCode));
        }

        public static new CollectionResult<T> Failure(ErrorCodes errorCode, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be null or empty", nameof(errorMessage));

            return new CollectionResult<T>(
                Array.Empty<T>(),
                new Error(errorMessage, (int)errorCode));
        }

        /// <summary>
        /// Получает коллекцию данных. 
        /// Возвращает пустую коллекцию, если операция завершилась ошибкой.
        /// </summary>
        public new IReadOnlyCollection<T> Data => base.Data ?? Array.Empty<T>();
    }
}
