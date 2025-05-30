namespace UsqlMcp.Application.Services;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models.Query;

public class QueryService
{
    private readonly IQueryRepository _queryRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly ILogger<QueryService> _logger;

    public QueryService(
        IQueryRepository queryRepository,
        IConnectionRepository connectionRepository,
        ILogger<QueryService> logger)
    {
        _queryRepository = queryRepository;
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public async Task<QueryResult?> ExecuteQueryAsync(string connectionId, string query, int maxRows = 100)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return null;
        }

        // Validate that the query is read-only
        var isValid = await _queryRepository.ValidateQueryAsync(connectionId, query);
        if (!isValid)
        {
            _logger.LogWarning("Query validation failed for connection {ConnectionId}", connectionId);
            throw new System.InvalidOperationException("Query validation failed. Only read-only queries are allowed.");
        }

        _logger.LogInformation("Executing query for connection: {ConnectionId}", connectionId);
        return await _queryRepository.ExecuteQueryAsync(connectionId, query, maxRows);
    }

    public async Task<QueryResult?> GetTableSampleAsync(string connectionId, string tableName, string? schema = null, int maxRows = 10)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return null;
        }

        // If schema is not provided, use the one from the connection if available
        schema ??= connection.SchemaName;

        _logger.LogInformation("Getting sample data for table {TableName} in schema {Schema} for connection: {ConnectionId}", 
            tableName, schema ?? "default", connectionId);
        return await _queryRepository.GetTableSampleAsync(connectionId, tableName, schema, maxRows);
    }
}