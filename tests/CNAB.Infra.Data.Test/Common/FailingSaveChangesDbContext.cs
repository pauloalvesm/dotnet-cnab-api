using CNAB.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CNAB.Infra.Data.Test.Common;

public class FailingSaveChangesDbContext : ApplicationDbContext
{
    public FailingSaveChangesDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new DbUpdateException("Simulated deletion exception");
    }
}