namespace UsqlMcp.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models.Query;
using UsqlMcp.Infrastructure.Services;

public class QueryRepository : IQueryRepository
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IQueryValidator _queryValidator;
    private readonly ILogger<QueryRepository> _logger;

    public QueryRepository(
        IDatabaseConnectionFactory connectionFactory,
        IQueryValidator queryValidator,
        ILogger<QueryRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _queryValidator = queryValidator;
        _logger = logger;
    }

    public async Task<bool> ValidateQueryAsync(string connectionId, string query)
    {
        return await _queryValidator.ValidateQueryAsync(connectionId, query);
    }

    public async Task<QueryResult> ExecuteQueryAsync(string connectionId, string query, int maxRows = 100)
    {
        using var connection = _connectionFactory.CreateConnection(connectionId);
        var stopwatch = new Stopwatch();

        try
        {
            // For non-DbConnection objects, open synchronously
            if (connection is DbConnection dbConnection)
            {
                await dbConnection.OpenAsync();
            }
            else
            {
                connection.Open();
            }
            
            stopwatch.Start();

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30; // Default timeout

            // For non-DbCommand objects, execute synchronously
            IDataReader reader;
            if (command is DbCommand dbCommand)
            {
                reader = await dbCommand.ExecuteReaderAsync();
            }
            else
            {
                reader = command.ExecuteReader();
            }
            
            using (reader)
            {
                var columns = new List<string>();
                var rows = new List<List<object?>>(); // Change to match QueryResult.Rows type
                var totalRows = 0;
                var truncated = false;

                // Get column names
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetName(i));
                }

                // Read rows
                while (reader.Read() && totalRows < maxRows)
                {
                    var row = new List<object?>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.IsDBNull(i) ? null : reader.GetValue(i));
                    }
                    rows.Add(row);
                    totalRows++;
                }

                // Check if there are more rows
                truncated = reader.Read();

                stopwatch.Stop();

                return new QueryResult
                {
                    Columns = columns,
                    Rows = rows, // This will now match the expected type
                    TotalRows = totalRows,
                    Truncated = truncated,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query for connection {ConnectionId}", connectionId);
            throw;
        }
        finally
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        }
    }

    public async Task<QueryResult> GetTableSampleAsync(string connectionId, string tableName, string? schema = null, int maxRows = 10)
    {
        // Build a simple SELECT query for the table
        var query = schema != null
            ? $"SELECT * FROM {schema}.{tableName} LIMIT {maxRows}"
            : $"SELECT * FROM {tableName} LIMIT {maxRows}";

        // Adjust LIMIT syntax based on database type
        var connection = _connectionFactory.CreateConnection(connectionId);
        if (connection is Microsoft.Data.SqlClient.SqlConnection)
        {
            query = schema != null
                ? $"SELECT TOP {maxRows} * FROM {schema}.{tableName}"
                : $"SELECT TOP {maxRows} * FROM {tableName}";
        }
        else if (connection is Oracle.ManagedDataAccess.Client.OracleConnection)
        {
            query = schema != null
                ? $"SELECT * FROM {schema}.{tableName} WHERE ROWNUM <= {maxRows}"
                : $"SELECT * FROM {tableName} WHERE ROWNUM <= {maxRows}";
        }

        return await ExecuteQueryAsync(connectionId, query, maxRows);
    }
}