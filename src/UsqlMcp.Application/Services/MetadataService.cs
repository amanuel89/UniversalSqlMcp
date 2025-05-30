namespace UsqlMcp.Application.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models.Metadata;

public class MetadataService
{
    private readonly IMetadataRepository _metadataRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly ILogger<MetadataService> _logger;

    public MetadataService(
        IMetadataRepository metadataRepository,
        IConnectionRepository connectionRepository,
        ILogger<MetadataService> logger)
    {
        _metadataRepository = metadataRepository;
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public async Task<DatabaseMetadata?> GetDatabaseMetadataAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return null;
        }

        _logger.LogInformation("Getting database metadata for connection: {ConnectionId}", connectionId);
        return await _metadataRepository.GetDatabaseMetadataAsync(connectionId);
    }

    public async Task<List<TableMetadata>> GetTablesAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return new List<TableMetadata>();
        }

        _logger.LogInformation("Getting tables for connection: {ConnectionId}", connectionId);
        return await _metadataRepository.GetTablesAsync(connectionId);
    }

    public async Task<TableMetadata?> GetTableMetadataAsync(string connectionId, string tableName, string? schema = null)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return null;
        }

        // If schema is not provided, use the one from the connection if available
        schema ??= connection.SchemaName;

        _logger.LogInformation("Getting metadata for table {TableName} in schema {Schema} for connection: {ConnectionId}", 
            tableName, schema ?? "default", connectionId);
        return await _metadataRepository.GetTableMetadataAsync(connectionId, tableName, schema);
    }

    // Methods for views
    public async Task<List<ViewMetadata>> GetViewsAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return new List<ViewMetadata>();
        }

        _logger.LogInformation("Getting views for connection: {ConnectionId}", connectionId);
        return await _metadataRepository.GetViewsAsync(connectionId);
    }

    public async Task<ViewMetadata?> GetViewMetadataAsync(string connectionId, string viewName, string? schema = null)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return null;
        }

        // If schema is not provided, use the one from the connection if available
        schema ??= connection.SchemaName;

        _logger.LogInformation("Getting metadata for view {ViewName} in schema {Schema} for connection: {ConnectionId}", 
            viewName, schema ?? "default", connectionId);
        return await _metadataRepository.GetViewMetadataAsync(connectionId, viewName, schema);
    }

    // Methods for stored procedures
    public async Task<List<StoredProcedureMetadata>> GetStoredProceduresAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return new List<StoredProcedureMetadata>();
        }

        _logger.LogInformation("Getting stored procedures for connection: {ConnectionId}", connectionId);
        return await _metadataRepository.GetStoredProceduresAsync(connectionId);
    }

    // Methods for functions
    public async Task<List<FunctionMetadata>> GetFunctionsAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return new List<FunctionMetadata>();
        }

        _logger.LogInformation("Getting functions for connection: {ConnectionId}", connectionId);
        return await _metadataRepository.GetFunctionsAsync(connectionId);
    }
}