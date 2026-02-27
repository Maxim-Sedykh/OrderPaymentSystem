using FluentValidation;
using OrderPaymentSystem.Shared.Result;

namespace OrderPaymentSystem.Application.Extensions;

/// <summary>
/// Расширение для построения правил в валидаторах FluentValidation
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Если правило нарушено, то отображать его с ошибкой, содержащей код и сообщение
    /// </summary>
    /// <typeparam name="T">Тип модели</typeparam>
    /// <typeparam name="TProperty">Свойство</typeparam>
    /// <param name="rule">Правило</param>
    /// <param name="error">Ошибка</param>
    /// <returns>Обновлённое правило</returns>
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Error error)
    {
        return rule
            .WithErrorCode(error.Code.ToString())
            .WithMessage(error.Message);
    }
}
