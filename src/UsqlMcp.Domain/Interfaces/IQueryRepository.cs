namespace UsqlMcp.Domain.Interfaces;

using System.Threading.Tasks;
using UsqlMcp.Domain.Models.Query;

public interface IQueryRepository
{
    Task<bool> ValidateQueryAsync(string connectionId, string query);
    Task<QueryResult> ExecuteQueryAsync(string connectionId, string query, int maxRows = 100);
    Task<QueryResult> ExecuteDirectQueryAsync(string connectionId, string query);
    Task<QueryResult> GetTableSampleAsync(string connectionId, string tableName, string? schema = null, int maxRows = 10);
}