using CNAB.Application.Interfaces;
using CNAB.Application.Interfaces.Account;
using CNAB.Application.Services;
using CNAB.Application.Services.Account;
using CNAB.Domain.Interfaces.Repositories;
using CNAB.Infra.Data.Context;
using CNAB.Infra.Data.Repositories;
using Mapster;
using MapsterMapper;
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
        services.AddDbContext<IdentityApplicationDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        var config = new TypeAdapterConfig();
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<ICNABProcessingService, CNABProcessingService>();
        services.AddScoped<ITokenService, TokenService>();
        
        return services;
    }
}