namespace OrderPaymentSystem.Application.Interfaces.Auth;

/// <summary>
/// Сервис для хэширования пароля
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Вернуть хэш по паролю
    /// </summary>
    /// <param name="password">Пароль</param>
    /// <returns>Хэш</returns>
    string Hash(string password);

    /// <summary>
    /// Сравнить введённый пароль и хранящийся в базе хэш пароля.
    /// Под капотом хэшируется введёный пароль и сравнивается с хэшем.
    /// </summary>
    /// <param name="enteredPassword">Введённый пароль</param>
    /// <param name="passwordHash">Хэш пароля</param>
    /// <returns>True - если введённый пароль и хэш пароля равны.</returns>
    bool Verify(string enteredPassword, string passwordHash);
}
