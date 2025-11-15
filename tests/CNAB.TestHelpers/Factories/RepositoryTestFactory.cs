using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;

namespace CNAB.TestHelpers.Factories;

public static class RepositoryTestFactory
{
    public static Store CreateStore(string name = "Default Store", string owner = "Default Owner")
    {
        return new Store(name, owner);
    }

    public static List<Store> GenerateListStores()
    {
        return new List<Store>
        {
            new Store("Store 1", "Owner 1"),
            new Store("Store 2", "Owner 2"),
            new Store("Store 3", "Owner 3")
        };
    }

    public static Transaction CreateTransaction(Store store = null)
    {
        store ??= CreateStore();

        return new Transaction(
            TransactionType.Credit,
            new DateTime(2024, 1, 1),
            100.00m,
            "12345678901",
            "1234****5678",
            new TimeSpan(12, 0, 0),
            store
        );
    }

    public static List<Transaction> GenerateListTransactions()
    {
        return new List<Transaction>
        {
            new Transaction(
                Guid.Parse("fbdeb885-f1bf-4551-9eb8-6431d13e380d"),
                TransactionType.Debit,
                DateTime.Now,
                1000.00m,
                "12345678901",
                "1234****5678",
                TimeSpan.FromHours(11),
                new Store("Store 1", "Location 1")
            ),
            new Transaction(
                Guid.Parse("1368323a-8608-4084-ab5e-99e0b1ccfa9b"),
                TransactionType.Credit,
                DateTime.Now,
                2000.00m,
                "98765432100",
                "1234****5678",
                TimeSpan.FromHours(12),
                new Store("Store 2", "Location 2")
            ),
            new Transaction(
                Guid.Parse("743e5c25-a3da-477d-adcf-fb7fdc1ac64a"),
                TransactionType.Credit,
                DateTime.Now,
                3000.00m,
                "98765432100",
                "1234****5678",
                TimeSpan.FromHours(13),
                new Store("Store 3", "Location 3")
            )
        };
    }
}