namespace UsqlMcp.Infrastructure;

using System.IO;
using Microsoft.Extensions.DependencyInjection;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Infrastructure.Repositories;
using UsqlMcp.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IConnectionRepository, ConnectionRepository>();
        services.AddScoped<IMetadataRepository, MetadataRepository>();
        services.AddScoped<IQueryRepository, QueryRepository>();
        services.AddScoped<ISemanticModelRepository, SemanticModelRepository>();

        // Register infrastructure services
        services.AddSingleton<ISqlScriptProvider, SqlScriptProvider>();
        services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
        services.AddScoped<IQueryValidator, QueryValidator>();

        return services;
    }

    // Helper method to copy SQL scripts to output directory
    public static void CopySqlScriptsToOutputDirectory(string contentRootPath, string outputPath)
    {
        var scriptsDestDir = Path.Combine(outputPath, "SqlScripts");

        if (!Directory.Exists(scriptsDestDir))
        {
            Directory.CreateDirectory(scriptsDestDir);
        }

        // The SQL scripts are now embedded in the Infrastructure project
        // and will be copied to the output directory during build
        // This method is kept for backward compatibility and to ensure
        // the scripts directory exists
    }
}