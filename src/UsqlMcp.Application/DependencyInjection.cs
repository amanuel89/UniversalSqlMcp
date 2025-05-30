namespace UsqlMcp.Application;

using Microsoft.Extensions.DependencyInjection;
using UsqlMcp.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ConnectionService>();
        services.AddScoped<MetadataService>();
        services.AddScoped<QueryService>();
        services.AddScoped<SemanticModelService>();
        
        return services;
    }
}