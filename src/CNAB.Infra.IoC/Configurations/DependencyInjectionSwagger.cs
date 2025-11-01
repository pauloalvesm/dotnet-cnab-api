using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CNAB.Infra.IoC.Configurations;

public static class DependencyInjectionSwagger
{
    public static IServiceCollection AddInfrastructureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CNAB - API",
                    Version = "v1",
                    Description = "API to manage and normalize CNAB transaction data.",
                    Contact = new OpenApiContact
                    {
                        Name = "Paulo Alves",
                        Email = "paulo.alves7351@gmail.com",
                        Url = new Uri("https://github.com/pauloalvesm"),
                    },
                });
            });

        return services;
    }
}