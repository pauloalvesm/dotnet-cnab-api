using CNAB.Application.DTOs;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CNAB.TestHelpers.Factories;

public static class ControllerTestFactory
{
    public static Store CreateStore()
    {
        return new Store("Test Store", "Test Owner");
    }

    public static StoreDto CreateStoreDto(Guid storeId)
    {
        return new StoreDto(storeId, "Test Store", "Test Owner", 1000.00m);
    }

    public static StoreInputDto CreateStoreInputDto()
    {
        return new StoreInputDto
        {
            Name = "New Store",
            OwnerName = "New Owner"
        };
    }

    public static StoreInputDto CreateStoreInputDto(Guid storeId)
    {
        return new StoreInputDto
        {
            Id = storeId,
            Name = "Updated Store Name",
            OwnerName = "Updated Owner Name"
        };
    }

    public static List<StoreDto> CreateStoreDtoList()
    {
        var storeId1 = new Guid("218bc11d-6167-4166-85a0-28842e1ab4bf");
        var storeId2 = new Guid("d5c78400-36f9-4ee3-8e85-04024f7495f3");
        var storeId3 = new Guid("93589716-b9ff-4604-827c-a9f9d81525e9");

        return new List<StoreDto>
        {
            new StoreDto(storeId1, "Store 1", "Owner 1", 1000.00m),
            new StoreDto(storeId2, "Store 2", "Owner 2", 2000.00m),
            new StoreDto(storeId3, "Store 3", "Owner 3", 3000.00m)
        };
    }

    public static TransactionDto CreateTransactionDto()
    {
        var transactionId1 = new Guid("d57166f3-f7d4-41bd-801c-135465718d20");
        var storeIdA = new Guid("fe61d9a8-f739-4db0-bd4d-fc1e13702d48");

        return new TransactionDto(
            id: transactionId1,
                type: (int)TransactionType.Debit,
                occurrenceDate: DateTime.Now.AddDays(-1),
                amount: 1000.00m,
                cPF: "12345678901",
                cardNumber: "1234****5678",
                time: TimeSpan.FromHours(11),
                storeId: storeIdA,
                storeName: "Store A",
                storeOwnerName: "Owner A"
        );
    }

    public static List<TransactionDto> GenerateListTransactions()
    {
        var transactionId1 = new Guid("9109a69c-0925-4f3c-9c70-441ad031f407");
        var transactionId2 = new Guid("d5c78400-36f9-4ee3-8e85-04024f7495f3");
        var transactionId3 = new Guid("93589716-b9ff-4604-827c-a9f9d81525e9");

        var storeIdA = new Guid("0a1b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c5d");
        var storeIdB = new Guid("1b2c3d4e-5f6a-7b8c-9d0e-1f2a3b4c5d6e");
        var storeIdC = new Guid("2c3d4e5f-6a7b-8c9d-0e1f-2a3b4c5d6e7f");


        return new List<TransactionDto>
        {
            new TransactionDto(
                id: transactionId1,
                type: (int)TransactionType.Debit,
                occurrenceDate: DateTime.Now.AddDays(-1),
                amount: 1000.00m,
                cPF: "12345678901",
                cardNumber: "1234****5678",
                time: TimeSpan.FromHours(11),
                storeId: storeIdA,
                storeName: "Store A",
                storeOwnerName: "Owner A"
            ),
            new TransactionDto(
                id: transactionId2,
                type: (int)TransactionType.Credit,
                occurrenceDate: DateTime.Now.AddDays(-2),
                amount: 2000.00m,
                cPF: "98765432100",
                cardNumber: "8765****4321",
                time: TimeSpan.FromHours(12),
                storeId: storeIdB,
                storeName: "Store B",
                storeOwnerName: "Owner B"
            ),
            new TransactionDto(
                id: transactionId3,
                type: (int)TransactionType.Credit,
                occurrenceDate: DateTime.Now.AddDays(-3),
                amount: 3000.00m,
                cPF: "11223344556",
                cardNumber: "9876****1234",
                time: TimeSpan.FromHours(13),
                storeId: storeIdC,
                storeName: "Store C",
                storeOwnerName: "Owner C"
            )
        };
    }

    public static TransactionDto UpdateTransactionDto(Guid transactionId)
    {
        var storeId = new Guid("fe61d9a8-f739-4db0-bd4d-fc1e13702d48");

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
            StoreName = "Updated Store",
            StoreOwnerName = "Updated Owner"
        };

        return transactionDto;
    }

    public static TransactionDto TransactionDtoNoId()
    {
        var storeId = new Guid("fe61d9a8-f739-4db0-bd4d-fc1e13702d48");

        var transactionDto = new TransactionDto
        {
            Id = Guid.NewGuid(),
            Type = (int)TransactionType.Credit,
            OccurrenceDate = DateTime.Now,
            Amount = 200m,
            CPF = "09876543210",
            CardNumber = "64722****315",
            Time = new TimeSpan(14, 45, 0),
            StoreId = storeId,
            StoreName = "Mismatch Store",
            StoreOwnerName = "Mismatch Owner"
        };

        return transactionDto;
    }

    public static Mock<UserManager<IdentityUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
    }

    public static Mock<SignInManager<IdentityUser>> MockSignInManager(Mock<UserManager<IdentityUser>> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

        return new Mock<SignInManager<IdentityUser>>(
            userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null,
            null,
            null,
            null
        );
    }
}