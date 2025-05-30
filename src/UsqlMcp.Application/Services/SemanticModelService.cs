namespace UsqlMcp.Application.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models.SemanticModel;

public class SemanticModelService
{
    private readonly ISemanticModelRepository _semanticModelRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly ILogger<SemanticModelService> _logger;

    public SemanticModelService(
        ISemanticModelRepository semanticModelRepository,
        IConnectionRepository connectionRepository,
        ILogger<SemanticModelService> logger)
    {
        _semanticModelRepository = semanticModelRepository;
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public async Task<SemanticModel?> GetSemanticModelAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return null;
        }

        _logger.LogInformation("Getting semantic model for connection: {ConnectionId}", connectionId);
        return await _semanticModelRepository.GetSemanticModelAsync(connectionId);
    }

    public async Task<SemanticTable?> GetTableSemanticModelAsync(string connectionId, string tableName)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return null;
        }

        _logger.LogInformation("Getting semantic model for table {TableName} in connection: {ConnectionId}", 
            tableName, connectionId);
        return await _semanticModelRepository.GetTableSemanticModelAsync(connectionId, tableName);
    }

    public async Task<Dictionary<string, GlossaryTerm>> GetBusinessGlossaryAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return new Dictionary<string, GlossaryTerm>();
        }

        _logger.LogInformation("Getting business glossary for connection: {ConnectionId}", connectionId);
        return await _semanticModelRepository.GetBusinessGlossaryAsync(connectionId);
    }

    public async Task<List<BusinessMetric>> GetBusinessMetricsAsync(string connectionId, string? businessArea = null)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return new List<BusinessMetric>();
        }

        _logger.LogInformation("Getting business metrics for connection: {ConnectionId}, business area: {BusinessArea}", 
            connectionId, businessArea ?? "all");
        return await _semanticModelRepository.GetBusinessMetricsAsync(connectionId, businessArea);
    }
}