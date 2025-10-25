namespace CNAB.Domain.Validations;

public class DomainExceptionValidation : Exception
{
    public DomainExceptionValidation()
    {
    }

    public DomainExceptionValidation(string error) : base(error) { }

    public static void GetErrors(bool hasErros, string error)
    {
        if (hasErros)
        {
            throw new DomainExceptionValidation(error);
        }
    }
}