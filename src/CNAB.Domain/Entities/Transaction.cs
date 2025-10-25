using CNAB.Domain.Entities.enums;
using CNAB.Domain.Validations;

namespace CNAB.Domain.Entities;

public class Transaction : Entity
{
    public TransactionType Type { get; private set; }
    public DateTime OccurrenceDate { get; private set; }
    public decimal Amount { get; private set; }
    public string CPF { get; private set; }
    public string CardNumber { get; private set; }
    public TimeSpan Time { get; private set; }
    public Store Store { get; private set; }

    public Transaction() { }

    public Transaction(
        TransactionType type,
        DateTime occurrenceDate,
        decimal amount,
        string cpf,
        string cardNumber,
        TimeSpan time,
        Store store)
        : this(Guid.NewGuid(), type, occurrenceDate, amount, cpf, cardNumber, time, store) { }

    public Transaction(
        Guid id,
        TransactionType type,
        DateTime occurrenceDate,
        decimal amount,
        string cpf,
        string cardNumber,
        TimeSpan time,
        Store store)
    {
        ValidateDomain(id, type, occurrenceDate, amount, cpf, cardNumber, time, store);
    }

    public decimal SignedAmount
    {
        get
        {
            return IsIncome ? Amount : -Amount;
        }
    }

    public bool IsExpense
    {
        get
        {
            return !IsIncome;
        }
    }

    public bool IsIncome
    {
        get
        {
            return Type == TransactionType.Debit ||
                            Type == TransactionType.Credit ||
                            Type == TransactionType.LoanReceipt ||
                            Type == TransactionType.Sales ||
                            Type == TransactionType.TEDReceipt ||
                            Type == TransactionType.DOCReceipt;
        }
    }

    public void UpdateDetails(
        TransactionType type,
        DateTime occurrenceDate,
        decimal amount,
        string cpf,
        string cardNumber,
        TimeSpan time)
    {
        ValidateDomain(Id, type, occurrenceDate, amount, cpf, cardNumber, time, Store);
        Type = type;
        OccurrenceDate = occurrenceDate;
        Amount = amount;
        CPF = cpf;
        CardNumber = cardNumber;
        Time = time;
    }

    private void ValidateDomain(
        Guid id,
        TransactionType type,
        DateTime occurrenceDate,
        decimal amount,
        string cpf,
        string cardNumber,
        TimeSpan time,
        Store store)
    {
        DomainExceptionValidation.GetErrors(id == Guid.Empty, "Invalid Id, Id cannot be empty");
        DomainExceptionValidation.GetErrors(!Enum.IsDefined(typeof(TransactionType), type), "Invalid type, Transaction type is required");
        DomainExceptionValidation.GetErrors(occurrenceDate == default(DateTime), "Invalid occurrenceDate, Date is required");
        DomainExceptionValidation.GetErrors(amount <= 0, "Invalid amount, must be greater than zero");
        DomainExceptionValidation.GetErrors(string.IsNullOrWhiteSpace(cpf), "Invalid CPF, CPF is required");
        DomainExceptionValidation.GetErrors(cpf.Length != 11, "Invalid CPF, must be 11 characters");
        DomainExceptionValidation.GetErrors(string.IsNullOrWhiteSpace(cardNumber), "Invalid card number, Card number is required");
        DomainExceptionValidation.GetErrors(store == null, "Invalid store, Store is required");

        Id = id;
        Type = type;
        OccurrenceDate = occurrenceDate;
        Amount = amount;
        CPF = cpf;
        CardNumber = cardNumber;
        Time = time;
        Store = store;
    }
}