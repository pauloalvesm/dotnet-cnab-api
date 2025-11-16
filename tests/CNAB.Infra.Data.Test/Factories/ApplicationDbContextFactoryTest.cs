using CNAB.Infra.Data.Factories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CNAB.Infra.Data.Test.Factories;

public class ApplicationDbContextFactoryTest
{
    [Fact(DisplayName = "CreateDbContext - Should Connect To The Database")]
    public void ApplicationDbContextFactory_CreateDbContext_ShouldConnectToDatabase()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var factory = new ApplicationDbContextFactory(serviceProvider, configuration);

        var dbContext = factory.CreateDbContext();

        dbContext.Database.OpenConnection();
        Assert.True(dbContext.Database.GetDbConnection().State == System.Data.ConnectionState.Open);

        dbContext.Database.CloseConnection();

        Assert.NotNull(dbContext);
    }

    [Fact(DisplayName = "CreateDbContext - Should Throw When Connection String Is Missing In Configuration")]
    public void ApplicationDbContextFactory_CreateDbContext_ShouldThrow_WhenConnectionStringIsMissing()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var factory = new ApplicationDbContextFactory(serviceProvider, configuration);

        var exception = Assert.Throws<InvalidOperationException>(() => factory.CreateDbContext());

        Assert.Contains("DefaultConnection", exception.Message);
    }
}