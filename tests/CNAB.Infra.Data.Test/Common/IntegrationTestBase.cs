using CNAB.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CNAB.Infra.Data.Test.Common;

public class IntegrationTestBase : IDisposable
{
    protected readonly ApplicationDbContext DbContext;

    public IntegrationTestBase()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .Build();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .Options;

        DbContext = new ApplicationDbContext(options);

        DbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Transactions\" RESTART IDENTITY CASCADE;");
        DbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Stores\" RESTART IDENTITY CASCADE;");
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}