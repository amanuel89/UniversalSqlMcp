namespace UsqlMcp.Domain.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using UsqlMcp.Domain.Models.Metadata;

public interface IMetadataRepository
{
    Task<DatabaseMetadata> GetDatabaseMetadataAsync(string connectionId);
    Task<List<TableMetadata>> GetTablesAsync(string connectionId);
    Task<TableMetadata?> GetTableMetadataAsync(string connectionId, string tableName, string? schema = null);
    Task<List<ViewMetadata>> GetViewsAsync(string connectionId);
    Task<ViewMetadata?> GetViewMetadataAsync(string connectionId, string viewName, string? schema = null);
    Task<List<StoredProcedureMetadata>> GetStoredProceduresAsync(string connectionId);
    Task<List<FunctionMetadata>> GetFunctionsAsync(string connectionId);
}