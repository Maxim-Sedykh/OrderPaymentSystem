using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.Interfaces.Validators;

/// <summary>
/// Интерфейс валидатора для авторизации пользователя
/// </summary>
public interface IAuthValidator
{
    /// <summary>
    /// Валидировать вход пользователя в аккаунт
    /// </summary>
    /// <param name="user">Пользователь, полученный по логину</param>
    /// <param name="enteredPassword">Введённый пользователем пароль</param>
    /// <returns>Результат валидации</returns>
    BaseResult ValidateLogin(User user, string enteredPassword);
}
