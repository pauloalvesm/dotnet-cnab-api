using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CNAB.Infra.Data.Context;
using CNAB.Infra.Data.Factories.Interfaces;

namespace CNAB.Infra.Data.Factories.Implementations;

public class ApplicationDbContextFactory : IApplicationDbContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public ApplicationDbContextFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public ApplicationDbContext CreateDbContext()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}