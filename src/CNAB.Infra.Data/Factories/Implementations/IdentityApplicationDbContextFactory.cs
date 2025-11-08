using CNAB.Infra.Data.Context;
using CNAB.Infra.Data.Factories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CNAB.Infra.Data.Factories.Implementations;

public class IdentityApplicationDbContextFactory : IIdentityApplicationDbContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public IdentityApplicationDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IdentityApplicationDbContext CreateDbContext()
    {
        var options = _serviceProvider.GetRequiredService<DbContextOptions<IdentityApplicationDbContext>>();
        return new IdentityApplicationDbContext(options);
    }
}