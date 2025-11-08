using CNAB.Domain.Validations;

namespace CNAB.Domain.Entities.Account;

public class User
{
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string ConfirmPassword { get; private set; }

    public User(string email, string password, string confirmPassword)
    {
        ValidateDomain(email, password, confirmPassword);
    }

    private void ValidateDomain(string email, string password, string confirmPassword)
    {
        DomainExceptionValidation.GetErrors(string.IsNullOrWhiteSpace(email), "Email is required.");
        DomainExceptionValidation.GetErrors(!email.Contains("@"), "Invalid email format.");
        DomainExceptionValidation.GetErrors(string.IsNullOrWhiteSpace(password), "Password is required.");
        DomainExceptionValidation.GetErrors(password.Length < 6, "Password must be at least 6 characters long.");
        DomainExceptionValidation.GetErrors(password != confirmPassword, "Passwords do not match.");

        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
}