namespace OrderPaymentSystem.Application.Interfaces.Auth;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string enteredPassword, string passwordHash);
}
