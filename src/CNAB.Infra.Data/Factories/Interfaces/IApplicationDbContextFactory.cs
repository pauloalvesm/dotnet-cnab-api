using CNAB.Infra.Data.Context;

namespace CNAB.Infra.Data.Factories.Interfaces;

public interface IApplicationDbContextFactory
{
    ApplicationDbContext CreateDbContext();
}