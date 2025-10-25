using CNAB.Domain.Validations;

namespace CNAB.Domain.Entities;

public class Store : Entity
{
    public string Name { get; private set; }
    public string OwnerName { get; private set; }
    private readonly List<Transaction> _transactions = new();

    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public Store() { }

    public Store(string name, string ownerName) : this(Guid.NewGuid(), name, ownerName) { }

    public Store(Guid id, string name, string ownerName)
    {
        ValidateDomain(id, name, ownerName);
    }

    public void AddTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
    }

    public decimal GetBalance()
    {
        return _transactions.Sum(t => t.SignedAmount);
    }

    public void UpdateDetails(string name, string ownerName)
    {
        ValidateDomain(Id, name, ownerName);
        Name = name;
        OwnerName = ownerName;
    }

    private void ValidateDomain(Guid id, string name, string ownerName)
    {
        DomainExceptionValidation.GetErrors(id == Guid.Empty, "Invalid Id, Id cannot be empty");
        DomainExceptionValidation.GetErrors(string.IsNullOrEmpty(name), "Invalid name, Name is required");
        DomainExceptionValidation.GetErrors(name.Length < 3, "Invalid name, too short, minimum 3 characters");
        DomainExceptionValidation.GetErrors(string.IsNullOrEmpty(ownerName), "Invalid owner name, Owner Name is required");
        DomainExceptionValidation.GetErrors(ownerName.Length < 3, "Invalid owner name, too short, minimum 3 characters");

        Id = id;
        Name = name;
        OwnerName = ownerName;
    }
}