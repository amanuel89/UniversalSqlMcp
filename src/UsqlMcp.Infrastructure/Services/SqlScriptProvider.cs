namespace UsqlMcp.Infrastructure.Services;

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Models;

public class SqlScriptProvider : ISqlScriptProvider
{
    private readonly ILogger<SqlScriptProvider> _logger;
    private readonly string _scriptsDirectory;

    public SqlScriptProvider(ILogger<SqlScriptProvider> logger)
    {
        _logger = logger;
        _scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlScripts");
    }

    public string GetMetadataScript(DatabaseType databaseType)
    {
        var scriptFileName = databaseType switch
        {
            DatabaseType.PostgreSQL => "postgresql_metadata.sql",
            DatabaseType.SQLServer => "sqlserver_metadata.sql",
            DatabaseType.MySQL => "mysql_metadata.sql",
            DatabaseType.Oracle => "oracle_metadata.sql",
            DatabaseType.SQLite => "sqlite_metadata.sql",
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported")
        };

        return LoadSqlScript(scriptFileName);
    }

    public string GetQueryValidatorScript(DatabaseType databaseType)
    {
        return LoadSqlScript("query_validator.sql");
    }

    private string LoadSqlScript(string scriptFileName)
    {
        var scriptPath = Path.Combine(_scriptsDirectory, scriptFileName);
        
        if (!File.Exists(scriptPath))
        {
            _logger.LogError("SQL script file not found: {ScriptPath}", scriptPath);
            throw new FileNotFoundException($"SQL script file not found: {scriptPath}");
        }

        return File.ReadAllText(scriptPath);
    }
}