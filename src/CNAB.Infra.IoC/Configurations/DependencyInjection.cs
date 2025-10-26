using CNAB.Domain.Interfaces.Repositories;
using CNAB.Infra.Data.Context;
using CNAB.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CNAB.Infra.IoC.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        return services;
    }
}