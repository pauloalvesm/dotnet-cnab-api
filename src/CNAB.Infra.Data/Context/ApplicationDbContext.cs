using CNAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CNAB.Infra.Data.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<Store> Stores { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}