namespace UsqlMcp.Infrastructure.Services;

using UsqlMcp.Domain.Models;

public interface ISqlScriptProvider
{
    string GetMetadataScript(DatabaseType databaseType);
    string GetQueryValidatorScript(DatabaseType databaseType);
}