using CNAB.Infra.Data.Context;
using CNAB.Infra.Data.Factories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CNAB.Infra.Data.Test.Factories;

public class IdentityApplicationDbContextFactoryTest
{
    [Fact(DisplayName = "CreateDbContext - Should connect to InMemory Identity DB")]
    public void IdentityApplicationDbContextFactory_CreateDbContext_ShouldConnectToInMemoryIdentityDatabase()
    {
        // Arrange: Configura o provedor de serviços com InMemory
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<IdentityApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("Test_Identity_Db");
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var factory = new IdentityApplicationDbContextFactory(serviceProvider);

        // Act: Cria o contexto real
        var context = factory.CreateDbContext();

        // Assert: Contexto criado e funcional
        Assert.NotNull(context);
        Assert.IsType<IdentityApplicationDbContext>(context);

        // Prepara e testa manipulação
        var user = new IdentityUser { UserName = "testuser", Email = "test@example.com" };
        context.Users.Add(user);
        context.SaveChanges();

        var fetchedUser = context.Users.FirstOrDefault(u => u.UserName == "testuser");
        Assert.NotNull(fetchedUser);
        Assert.Equal("test@example.com", fetchedUser.Email);
    }

    [Fact(DisplayName = "CreateDbContext - Should throw when options not configured")]
    public void IdentityApplicationDbContextFactory_CreateDbContext_ShouldThrowWhenOptionsNotConfigured()
    {
        // Arrange
        var emptyServiceProvider = new ServiceCollection().BuildServiceProvider();

        var factory = new IdentityApplicationDbContextFactory(emptyServiceProvider);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => factory.CreateDbContext());
        Assert.Contains("No service for type", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}