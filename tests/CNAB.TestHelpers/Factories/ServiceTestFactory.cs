using CNAB.Application.DTOs;
using CNAB.Application.DTOs.Account;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.Account;
using CNAB.Domain.Entities.enums;

namespace CNAB.TestHelpers.Factories;

public static class ServiceTestFactory
{
    public static List<Store> GenerateMockStores()
    {
        return new List<Store>
        {
            new Store("Store A", "Owner A"),
            new Store("Store B", "Owner B"),
            new Store("Store C", "Owner C")
        };
    }

    public static Store CreateStore()
    {
        return new Store("JOHN'S Bar", "JOHN DOE");
    }

    public static List<StoreDto> GenerateStoresWithPositiveBalances()
    {
        return CreateStoreListWithBalances(
            ("Store 1", 1000.50m),
            ("Store 2", 2500.75m),
            ("Store 3", 750.25m)
        );
    }

    public static List<StoreDto> GenerateStoresWithNegativeBalances()
    {
        return CreateStoreListWithBalances(
            ("Store 1", -100.00m),
            ("Store 2", -200.00m),
            ("Store 3", -150.00m)
        );
    }

    public static List<StoreDto> GenerateStoresWithPositiveandNegativeBalances()
    {
        return CreateStoreListWithBalances(
            ("Store 1", 1000.00m),
            ("Store 2", -500.00m),
            ("Store 3", 250.00m)
        );
    }

    public static StoreInputDto CreateStoreInputDto()
    {
        return new StoreInputDto
        {
            Name = "New Store",
            OwnerName = "New Owner"
        };
    }

    public static StoreInputDto UpdateStoreInputDto(Guid storeId)
    {
        return new StoreInputDto
        {
            Id = storeId,
            Name = "Updated Store",
            OwnerName = "Updated Owner"
        };
    }

    public static StoreDto CreateStoreDto(Guid storeId, string name, string ownerName)
    {
        return new StoreDto
        {
            Id = storeId,
            Name = name,
            OwnerName = ownerName,
            Balance = 0m
        };
    }

    public static Transaction CreateTransaction()
    {
        var store = CreateStore();

        return new Transaction(
            type: TransactionType.Debit,
            occurrenceDate: new DateTime(2019, 3, 1, 15, 34, 53),
            amount: 142.00m,
            cpf: "00962067601",
            cardNumber: "74753****3153",
            time: new TimeSpan(15, 34, 53),
            store: store
        );
    }

    public static Transaction CreateTransaction(Guid id)
    {
        var store = new Store("Test Store", "Test Owner");

        var transaction = new Transaction(
            TransactionType.Debit,
            DateTime.Now,
            100m,
            "12345678901",
            "1234567890123456",
            new TimeSpan(10, 30, 0),
            store);

        typeof(Entity).GetProperty("Id").SetValue(transaction, id);

        return transaction;
    }

    public static Transaction CreateTransaction(Store store)
    {
        return new Transaction(
            TransactionType.Debit,
            DateTime.Now,
            100m,
            "12345678901",
            "1234567890123456",
            new TimeSpan(10, 30, 0),
            store);
    }

    public static List<TransactionDto> CreateTransactionList(int count)
    {
        var transactions = new List<TransactionDto>();

        for (int i = 0; i < count; i++)
        {
            var transaction = CreateTransactionDto(Guid.NewGuid());
            transaction.Amount = (i + 1) * 100m;
            transactions.Add(transaction);
        }

        return transactions;
    }

    public static Transaction UpdateTransaction(Guid storeId, Store store)
    {
        var transaction = new Transaction(
            TransactionType.Debit,
            DateTime.Now.AddDays(-1),
            100m,
            "12345678901",
            "1234567890123456",
            new TimeSpan(10, 30, 0),
            store);

        return transaction;
    }

    public static TransactionDto CreateTransactionDto(Guid id)
    {
        return new TransactionDto
        {
            Id = id,
            Type = (int)TransactionType.Debit,
            OccurrenceDate = DateTime.Now,
            Amount = 100m,
            CPF = "12345678901",
            CardNumber = "1234567890123456",
            Time = new TimeSpan(10, 30, 0),
            StoreId = Guid.NewGuid(),
            StoreName = "Test Store",
            StoreOwnerName = "Test Owner"
        };
    }

    public static TransactionDto CreateTransactionDtoInvalidEnum()
    {
        return new TransactionDto
        {
            Id = new Guid("218bc11d-6167-4166-85a0-28842e1ab4bf"),
            Type = 999,
            OccurrenceDate = DateTime.Now,
            Amount = 100m,
            CPF = "12345678901",
            CardNumber = "64722****315",
            Time = new TimeSpan(10, 30, 0),
            StoreId = Guid.NewGuid(),
            StoreName = "Test Store",
            StoreOwnerName = "Test Owner"
        };
    }

    public static TransactionDto UpdateTransactionDto(Guid transactionId, Guid storeId)
    {
        var transactionDto = new TransactionDto
        {
            Id = transactionId,
            Type = (int)TransactionType.Credit,
            OccurrenceDate = DateTime.Now,
            Amount = 200m,
            CPF = "09876543210",
            CardNumber = "64722****315",
            Time = new TimeSpan(14, 45, 0),
            StoreId = storeId,
            StoreName = "Test Store",
            StoreOwnerName = "Test Owner"
        };

        return transactionDto;
    }

    public static string GetValidCnabLinePadded()
    {
        return GetValidCnabLine().PadRight(81, ' ');
    }

    public static string GetValidCnabLine()
    {
        return "1" +
               "20190301" +
               "0000014200" +
               "00962067601" +
               "74753****315" +
               "153453" +
               "JOHN DOE      " +
               "JOHN'S Bar         ";
    }

    public static User CreateUser()
    {
        return new User(
            email: "testuser@example.com",
            password: "123456",
            confirmPassword: "123456"
        );
    }

    public static UserDto CreateUserDto()
    {
        return new UserDto
        {
            Email = "testuserdto@example.com",
            Password = "123456",
            ConfirmPassword = "123456"
        };
    }

    public static Login CreateLogin()
    {
        return new Login(
            email: "login@example.com",
            password: "123456"
        );
    }
    public static UserToken CreateUserToken()
    {
        return new UserToken
        {
            Authenticated = true,
            Expiration = DateTime.Now.AddHours(1),
            Token = "sometoken",
            Message = "Authentication successful"
        };
    }

    private static List<StoreDto> CreateStoreListWithBalances(params (string name, decimal balance)[] storeData)
    {
        var stores = new List<StoreDto>();

        for (int i = 0; i < storeData.Length; i++)
        {
            var (name, balance) = storeData[i];
            var store = CreateStoreDto(Guid.NewGuid(), name, $"Owner {i + 1}");
            store.Balance = balance;
            stores.Add(store);
        }

        return stores;
    }
}