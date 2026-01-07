using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Extensions
{
    public static class ResultExtensions
    {
        public static BaseResult Failure(this BaseResult _, ErrorCodes errorCode, string errorMessage)
        {
            return BaseResult.Failure(new Error(errorMessage, (int)errorCode));
        }

        public static DataResult<T> Failure<T>(this DataResult<T> _, ErrorCodes errorCode, string errorMessage)
        {
            return DataResult<T>.Failure(new Error(errorMessage, (int)errorCode));
        }
    }
}
