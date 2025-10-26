namespace CNAB.Application.DTOs;

public class StoreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; }

    public StoreDto() { }

    public StoreDto(Guid id, string name, string ownerName, decimal balance)
    {
        Id = id;
        Name = name;
        OwnerName = ownerName;
        Balance = balance;
    }
}