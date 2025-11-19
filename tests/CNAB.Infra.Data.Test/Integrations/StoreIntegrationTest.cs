using CNAB.Domain.Entities;
using CNAB.Domain.Validations;
using CNAB.Infra.Data.Test.Common;
using FluentAssertions;

namespace CNAB.Infra.Data.Test.Integrations;

public class StoreIntegrationTest : IntegrationTestBase
{
    [Fact(DisplayName = "ApplicationDbContext - Can insert Store")]
    public void ApplicationDbContext_CanInsertStore()
    {
        // Arrange
        var store = new Store("Test Store", "Test Owner");

        // Act
        DbContext.Stores.Add(store);
        DbContext.SaveChanges();

        // Assert
        var insertedStore = DbContext.Stores.FirstOrDefault(s => s.Name == "Test Store");
        insertedStore.Should().NotBeNull();
        insertedStore.Name.Should().Be("Test Store");
        insertedStore.OwnerName.Should().Be("Test Owner");
        insertedStore.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "ApplicationDbContext - Can query Store by Id")]
    public void ApplicationDbContext_CanQueryStoreById()
    {
        // Arrange
        var store = new Store("Loja Query Test", "Owner Query Test");
        DbContext.Stores.Add(store);
        DbContext.SaveChanges();

        // Act
        var retrievedStore = DbContext.Stores.Find(store.Id);

        // Assert
        retrievedStore.Should().NotBeNull();
        retrievedStore.Name.Should().Be("Loja Query Test");
        retrievedStore.OwnerName.Should().Be("Owner Query Test");
    }

    [Fact(DisplayName = "ApplicationDbContext - Cannot insert Store with null Name due to Domain Validation")]
    public void ApplicationDbContext_CannotInsertStore_WithNullNameDueToDomainValidation()
    {
        // Arrange
        string invalidName = null;
        string validOwnerName = "Valid Owner";

        // Act
        Action act = () =>
        {
            var store = new Store(invalidName, validOwnerName);
            DbContext.Stores.Add(store);
            DbContext.SaveChanges();
        };

        // Assert
        act.Should().Throw<DomainExceptionValidation>()
           .WithMessage("Invalid name, Name is required");
        DbContext.Stores.Should().BeEmpty();
    }

    [Fact(DisplayName = "ApplicationDbContext - Cannot insert Store with empty Name due to Domain Validation")]
    public void ApplicationDbContext_CannotInsertStore_WithEmptyNameDueToDomainValidation()
    {
        // Arrange
        string invalidName = string.Empty;
        string validOwnerName = "Valid Owner";

        // Act
        Action act = () =>
        {
            var store = new Store(invalidName, validOwnerName);
            DbContext.Stores.Add(store);
            DbContext.SaveChanges();
        };

        // Assert
        act.Should().Throw<DomainExceptionValidation>()
           .WithMessage("Invalid name, Name is required");
        DbContext.Stores.Should().BeEmpty();
    }
}