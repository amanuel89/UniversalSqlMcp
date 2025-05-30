namespace UsqlMcp.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models.Metadata;
using UsqlMcp.Infrastructure.Services;
using System.Text;

public class MetadataRepository : IMetadataRepository
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly ISqlScriptProvider _sqlScriptProvider;
    private readonly ILogger<MetadataRepository> _logger;
    private readonly IConnectionRepository _connectionRepository;

    public MetadataRepository(
        IDatabaseConnectionFactory connectionFactory,
        ISqlScriptProvider sqlScriptProvider,
        ILogger<MetadataRepository> logger,
        IConnectionRepository connectionRepository)
    {
        _connectionFactory = connectionFactory;
        _sqlScriptProvider = sqlScriptProvider;
        _logger = logger;
        _connectionRepository = connectionRepository;
    }

    public async Task<DatabaseMetadata> GetDatabaseMetadataAsync(string connectionId)
    {
        using var connection = _connectionFactory.CreateConnection(connectionId);
        var dbConnection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        
        if (dbConnection == null)
        {
            throw new ArgumentException($"Connection with ID {connectionId} not found");
        }
    
        var metadata = new DatabaseMetadata
        {
            DatabaseName = connection.Database,
            Tables = await GetTablesAsync(connectionId),
            Views = await GetViewsAsync(connectionId),
            StoredProcedures = await GetStoredProceduresAsync(connectionId),
            Functions = await GetFunctionsAsync(connectionId)
        };
    
        return metadata;
    }

    public async Task<List<TableMetadata>> GetTablesAsync(string connectionId)
    {
        using var connection = _connectionFactory.CreateConnection(connectionId);
        var dbConnection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        
        if (dbConnection == null)
        {
            throw new ArgumentException($"Connection with ID {connectionId} not found");
        }

        var script = _sqlScriptProvider.GetMetadataScript(dbConnection.DatabaseType);
        var tables = new List<TableMetadata>();

        try
        {
            // IDbConnection doesn't have OpenAsync, so handle both sync and async cases
            if (connection is DbConnection dbConn)
            {
                await dbConn.OpenAsync();
            }
            else
            {
                connection.Open();
            }

            // Extract tables query based on database type
            string tablesQuery = ExtractQueryFromScript(script, "Tables Metadata");
            var tableResults = await connection.QueryAsync<dynamic>(tablesQuery);

            foreach (var table in tableResults)
            {
                var tableName = GetPropertyValue<string>(table, "table_name");
                var tableMetadata = new TableMetadata
                {
                    Name = tableName,
                    Description = GetPropertyValue<string>(table, "table_description"),
                    Columns = await GetColumnsForTableAsync(connection, tableName, dbConnection.SchemaName),
                    ForeignKeys = await GetForeignKeysForTableAsync(connection, tableName, dbConnection.SchemaName),
                    Indexes = await GetIndexesForTableAsync(connection, tableName, dbConnection.SchemaName),
                    PrimaryKeyColumns = ConvertPrimaryKeysToColumnNames(await GetPrimaryKeysForTableAsync(connection, tableName, dbConnection.SchemaName))
                };

                tables.Add(tableMetadata);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tables for connection {ConnectionId}", connectionId);
            throw;
        }

        return tables;
    }

    public async Task<TableMetadata?> GetTableMetadataAsync(string connectionId, string tableName, string? schema = null)
    {
        var tables = await GetTablesAsync(connectionId);
        return tables.Find(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<ViewMetadata>> GetViewsAsync(string connectionId)
    {
        using var connection = _connectionFactory.CreateConnection(connectionId);
        var dbConnection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        
        if (dbConnection == null)
        {
            throw new ArgumentException($"Connection with ID {connectionId} not found");
        }

        var script = _sqlScriptProvider.GetMetadataScript(dbConnection.DatabaseType);
        var views = new List<ViewMetadata>();

        try
        {
            // IDbConnection doesn't have OpenAsync, so handle both sync and async cases
            if (connection is DbConnection dbConn)
            {
                await dbConn.OpenAsync();
            }
            else
            {
                connection.Open();
            }

            // Extract views query based on database type
            string viewsQuery = ExtractQueryFromScript(script, "Views");
            var viewResults = await connection.QueryAsync<dynamic>(viewsQuery);

            foreach (var view in viewResults)
            {
                var viewMetadata = new ViewMetadata
                {
                    Name = GetPropertyValue<string>(view, "view_name"),
                    Definition = GetPropertyValue<string>(view, "view_definition")
                };

                views.Add(viewMetadata);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting views for connection {ConnectionId}", connectionId);
            throw;
        }

        return views;
    }

    public async Task<ViewMetadata?> GetViewMetadataAsync(string connectionId, string viewName, string? schema = null)
    {
        var views = await GetViewsAsync(connectionId);
        return views.Find(v => v.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<StoredProcedureMetadata>> GetStoredProceduresAsync(string connectionId)
    {
        using var connection = _connectionFactory.CreateConnection(connectionId);
        var dbConnection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        
        if (dbConnection == null)
        {
            throw new ArgumentException($"Connection with ID {connectionId} not found");
        }

        var script = _sqlScriptProvider.GetMetadataScript(dbConnection.DatabaseType);
        var procedures = new List<StoredProcedureMetadata>();

        try
        {
            // IDbConnection doesn't have OpenAsync, so handle both sync and async cases
            if (connection is DbConnection dbConn)
            {
                await dbConn.OpenAsync();
            }
            else
            {
                connection.Open();
            }

            // Extract stored procedures query based on database type
            string proceduresQuery = ExtractQueryFromScript(script, "Stored Procedures");
            var procedureResults = await connection.QueryAsync<dynamic>(proceduresQuery);

            foreach (var procedure in procedureResults)
            {
                var procedureMetadata = new StoredProcedureMetadata
                {
                    Name = GetPropertyValue<string>(procedure, "procedure_name"),
                    Definition = GetPropertyValue<string>(procedure, "procedure_definition"),
                    Description = GetPropertyValue<string>(procedure, "procedure_description")
                };

                procedures.Add(procedureMetadata);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stored procedures for connection {ConnectionId}", connectionId);
            throw;
        }

        return procedures;
    }

    public async Task<List<FunctionMetadata>> GetFunctionsAsync(string connectionId)
    {
        using var connection = _connectionFactory.CreateConnection(connectionId);
        var dbConnection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        
        if (dbConnection == null)
        {
            throw new ArgumentException($"Connection with ID {connectionId} not found");
        }

        var script = _sqlScriptProvider.GetMetadataScript(dbConnection.DatabaseType);
        var functions = new List<FunctionMetadata>();

        try
        {
            // IDbConnection doesn't have OpenAsync, so handle both sync and async cases
            if (connection is DbConnection dbConn)
            {
                await dbConn.OpenAsync();
            }
            else
            {
                connection.Open();
            }

            // Extract functions query based on database type
            string functionsQuery = ExtractQueryFromScript(script, "Functions");
            var functionResults = await connection.QueryAsync<dynamic>(functionsQuery);

            foreach (var function in functionResults)
            {
                var functionMetadata = new FunctionMetadata
                {
                    Name = GetPropertyValue<string>(function, "function_name"),
                    Definition = GetPropertyValue<string>(function, "function_definition"),
                    Description = GetPropertyValue<string>(function, "function_description")
                };

                functions.Add(functionMetadata);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting functions for connection {ConnectionId}", connectionId);
            throw;
        }

        return functions;
    }

    // Helper methods for extracting data
    private string ExtractQueryFromScript(string script, string sectionName)
    {
        // Simple extraction based on comment markers
        var lines = script.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var inSection = false;
        var query = new StringBuilder();

        foreach (var line in lines)
        {
            if (line.Contains($"-- {sectionName}") || line.Contains($"/* {sectionName}"))
            {
                inSection = true;
                continue;
            }

            if (inSection && (line.StartsWith("--") && !line.Contains("FROM") && !line.Contains("WHERE") && !line.Contains("JOIN")))
            {
                // End of section
                break;
            }

            if (inSection && !line.StartsWith("--") && !string.IsNullOrWhiteSpace(line))
            {
                query.AppendLine(line);
            }
        }

        return query.ToString();
    }

    private T? GetPropertyValue<T>(dynamic obj, string propertyName)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(propertyName, out var value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            // Try case-insensitive lookup
            foreach (var key in dict.Keys)
            {
                if (key.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)Convert.ChangeType(dict[key], typeof(T));
                }
            }
        }

        try
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop != null)
            {
                return (T)prop.GetValue(obj);
            }
        }
        catch
        {
            // Property doesn't exist or can't be accessed
        }

        return default;
    }

    // Helper methods for getting table details
    private async Task<List<ColumnMetadata>> GetColumnsForTableAsync(IDbConnection connection, string tableName, string? schema)
    {
        var columns = new List<ColumnMetadata>();
        try
        {
            // SQL for SQL Server to get column information
            var query = @"
                SELECT 
                    c.name AS column_name,
                    t.name AS data_type,
                    c.max_length,
                    c.precision,
                    c.scale,
                    c.is_nullable,
                    c.is_identity,
                    ep.value AS description,
                    c.column_id AS ordinal_position
                FROM 
                    sys.columns c
                INNER JOIN 
                    sys.types t ON c.user_type_id = t.user_type_id
                INNER JOIN 
                    sys.tables tbl ON c.object_id = tbl.object_id
                LEFT JOIN 
                    sys.extended_properties ep ON ep.major_id = c.object_id AND ep.minor_id = c.column_id AND ep.name = 'MS_Description'
                WHERE 
                    tbl.name = @tableName
                    AND (@schema IS NULL OR SCHEMA_NAME(tbl.schema_id) = @schema)
                ORDER BY 
                    c.column_id";

            var columnResults = await connection.QueryAsync<dynamic>(query, new { tableName, schema });

            foreach (var col in columnResults)
            {
                columns.Add(new ColumnMetadata
                {
                    Name = GetPropertyValue<string>(col, "column_name"),
                    DataType = GetPropertyValue<string>(col, "data_type"),
                    CharacterMaximumLength = GetPropertyValue<int>(col, "max_length"),
                    NumericPrecision = GetPropertyValue<int>(col, "precision"),
                    NumericScale = GetPropertyValue<int>(col, "scale"),
                    IsNullable = GetPropertyValue<bool>(col, "is_nullable"),
                    IsPrimaryKey = false, // Will be set later based on primary key information
                    IsForeignKey = false, // Will be set later based on foreign key information
                    Description = GetPropertyValue<string>(col, "description"),
                    OrdinalPosition = GetPropertyValue<int>(col, "ordinal_position")
                });
            }

            _logger.LogInformation("Retrieved {Count} columns for table {TableName}", columns.Count, tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting columns for table {TableName}", tableName);
        }

        return columns;
    }

    private async Task<List<PrimaryKeyMetadata>> GetPrimaryKeysForTableAsync(IDbConnection connection, string tableName, string? schema)
    {
        var primaryKeys = new List<PrimaryKeyMetadata>();
        try
        {
            // SQL for SQL Server to get primary key information
            var query = @"
                SELECT 
                    c.name AS column_name,
                    i.name AS constraint_name,
                    ic.key_ordinal AS key_ordinal
                FROM 
                    sys.indexes i
                INNER JOIN 
                    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN 
                    sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                INNER JOIN 
                    sys.tables t ON i.object_id = t.object_id
                WHERE 
                    i.is_primary_key = 1
                    AND t.name = @tableName
                    AND (@schema IS NULL OR SCHEMA_NAME(t.schema_id) = @schema)
                ORDER BY 
                    ic.key_ordinal";

            var pkResults = await connection.QueryAsync<dynamic>(query, new { tableName, schema });

            foreach (var pk in pkResults)
            {
                primaryKeys.Add(new PrimaryKeyMetadata
                {
                    Name = GetPropertyValue<string>(pk, "constraint_name"),
                    ColumnName = GetPropertyValue<string>(pk, "column_name"),
                    OrdinalPosition = GetPropertyValue<int>(pk, "key_ordinal")
                });
            }

            _logger.LogInformation("Retrieved {Count} primary key columns for table {TableName}", primaryKeys.Count, tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting primary keys for table {TableName}", tableName);
        }

        return primaryKeys;
    }

    private async Task<List<ForeignKeyMetadata>> GetForeignKeysForTableAsync(IDbConnection connection, string tableName, string? schema)
    {
        var foreignKeys = new List<ForeignKeyMetadata>();
        try
        {
            // SQL for SQL Server to get foreign key information
            var query = @"
                SELECT 
                    fk.name AS constraint_name,
                    OBJECT_NAME(fk.referenced_object_id) AS referenced_table,
                    c1.name AS column_name,
                    c2.name AS referenced_column,
                    fk.delete_referential_action AS delete_rule,
                    fk.update_referential_action AS update_rule
                FROM 
                    sys.foreign_keys fk
                INNER JOIN 
                    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                INNER JOIN 
                    sys.tables t ON fk.parent_object_id = t.object_id
                INNER JOIN 
                    sys.columns c1 ON fkc.parent_object_id = c1.object_id AND fkc.parent_column_id = c1.column_id
                INNER JOIN 
                    sys.columns c2 ON fkc.referenced_object_id = c2.object_id AND fkc.referenced_column_id = c2.column_id
                WHERE 
                    t.name = @tableName
                    AND (@schema IS NULL OR SCHEMA_NAME(t.schema_id) = @schema)";

            var fkResults = await connection.QueryAsync<dynamic>(query, new { tableName, schema });

            foreach (var fk in fkResults)
            {
                // Check if we already have a foreign key with this name
                var existingFk = foreignKeys.Find(f => f.Name == GetPropertyValue<string>(fk, "constraint_name"));
                
                if (existingFk != null)
                {
                    // Add a new column pair to the existing foreign key
                    existingFk.ColumnPairs.Add(new ForeignKeyColumnPair
                    {
                        ColumnName = GetPropertyValue<string>(fk, "column_name"),
                        ReferencedColumnName = GetPropertyValue<string>(fk, "referenced_column")
                    });
                }
                else
                {
                    // Create a new foreign key
                    var newFk = new ForeignKeyMetadata
                    {
                        Name = GetPropertyValue<string>(fk, "constraint_name"),
                        ReferencedTableName = GetPropertyValue<string>(fk, "referenced_table"),
                        ReferencedTableSchema = schema, // Using the provided schema
                        ColumnPairs = new List<ForeignKeyColumnPair>
                        {
                            new ForeignKeyColumnPair
                            {
                                ColumnName = GetPropertyValue<string>(fk, "column_name"),
                                ReferencedColumnName = GetPropertyValue<string>(fk, "referenced_column")
                            }
                        }
                    };
                    
                    foreignKeys.Add(newFk);
                }
            }

            _logger.LogInformation("Retrieved {Count} foreign keys for table {TableName}", foreignKeys.Count, tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting foreign keys for table {TableName}", tableName);
        }

        return foreignKeys;
    }

    private string GetDeleteRule(int actionValue)
    {
        return actionValue switch
        {
            0 => "NO ACTION",
            1 => "CASCADE",
            2 => "SET NULL",
            3 => "SET DEFAULT",
            _ => "UNKNOWN"
        };
    }

    private string GetUpdateRule(int actionValue)
    {
        return actionValue switch
        {
            0 => "NO ACTION",
            1 => "CASCADE",
            2 => "SET NULL",
            3 => "SET DEFAULT",
            _ => "UNKNOWN"
        };
    }

    private async Task<List<IndexMetadata>> GetIndexesForTableAsync(IDbConnection connection, string tableName, string? schema)
    {
        var indexes = new List<IndexMetadata>();
        try
        {
            // SQL for SQL Server to get index information
            var query = @"
                SELECT 
                    i.name AS index_name,
                    i.is_unique,
                    c.name AS column_name,
                    ic.key_ordinal
                FROM 
                    sys.indexes i
                INNER JOIN 
                    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN 
                    sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                INNER JOIN 
                    sys.tables t ON i.object_id = t.object_id
                WHERE 
                    i.is_primary_key = 0 -- Exclude primary keys
                    AND t.name = @tableName
                    AND (@schema IS NULL OR SCHEMA_NAME(t.schema_id) = @schema)
                ORDER BY 
                    i.name, ic.key_ordinal";

            var indexResults = await connection.QueryAsync<dynamic>(query, new { tableName, schema });

            // Group by index name to handle multi-column indexes
            var indexGroups = indexResults.GroupBy(idx => GetPropertyValue<string>(idx, "index_name"));

            foreach (var group in indexGroups)
            {
                if (group.Any())
                {
                    var firstItem = group.First();
                    var indexMetadata = new IndexMetadata
                    {
                        Name = group.Key,
                        IsUnique = GetPropertyValue<bool>(firstItem, "is_unique"),
                        ColumnNames = new List<string>()
                    };

                    // Add columns in the correct order
                    foreach (var item in group.OrderBy(i => GetPropertyValue<int>(i, "key_ordinal")))
                    {
                        indexMetadata.ColumnNames.Add(GetPropertyValue<string>(item, "column_name"));
                    }

                    indexes.Add(indexMetadata);
                }
            }

            _logger.LogInformation("Retrieved {Count} indexes for table {TableName}", indexes.Count, tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting indexes for table {TableName}", tableName);
        }

        return indexes;
    }

    // Helper method to convert PrimaryKeyMetadata to column names
    private List<string> ConvertPrimaryKeysToColumnNames(List<PrimaryKeyMetadata> primaryKeys)
    {
        var columnNames = new List<string>();
        foreach (var pk in primaryKeys)
        {
            if (!string.IsNullOrEmpty(pk.ColumnName))
            {
                columnNames.Add(pk.ColumnName);
            }
        }
        return columnNames;
    }
}
