using CNAB.Domain.Validations;

namespace CNAB.Domain.Entities.Account;

public class Login
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    public Login(string email, string password)
    {
        ValidateDomain(email, password);
    }

    private void ValidateDomain(string email, string password)
    {
        DomainExceptionValidation.GetErrors(string.IsNullOrWhiteSpace(email), "Email is required.");
        DomainExceptionValidation.GetErrors(!email.Contains("@"), "Invalid email format.");
        DomainExceptionValidation.GetErrors(string.IsNullOrWhiteSpace(password), "Password is required.");

        Email = email;
        Password = password;
    }
}