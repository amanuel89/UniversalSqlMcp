namespace UsqlMcp.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models;
using UsqlMcp.Domain.Models.SemanticModel;

public class SemanticModelRepository : ISemanticModelRepository
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ILogger<SemanticModelRepository> _logger;

    public SemanticModelRepository(
        IConnectionRepository connectionRepository,
        ILogger<SemanticModelRepository> logger)
    {
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public async Task<SemanticModel?> GetSemanticModelAsync(string connectionId)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null || string.IsNullOrEmpty(connection.SemanticModelPath))
        {
            return null;
        }

        try
        {
            if (!File.Exists(connection.SemanticModelPath))
            {
                _logger.LogWarning("Semantic model file not found: {FilePath}", connection.SemanticModelPath);
                return null;
            }

            var yaml = await File.ReadAllTextAsync(connection.SemanticModelPath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var semanticModel = deserializer.Deserialize<SemanticModel>(yaml);
            return semanticModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading semantic model from {FilePath}", connection.SemanticModelPath);
            return null;
        }
    }

    public async Task<SemanticTable?> GetTableSemanticModelAsync(string connectionId, string tableName)
    {
        var semanticModel = await GetSemanticModelAsync(connectionId);
        if (semanticModel == null || semanticModel.Tables == null)
        {
            return null;
        }

        // Use TryGetValue instead of Find for Dictionary
        if (semanticModel.Tables.TryGetValue(tableName, out var table))
        {
            return table;
        }
        
        // If case-insensitive lookup is needed
        foreach (var kvp in semanticModel.Tables)
        {
            if (kvp.Key.Equals(tableName, StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }
        
        return null;
    }

    public async Task<Dictionary<string, GlossaryTerm>> GetBusinessGlossaryAsync(string connectionId)
    {
        var semanticModel = await GetSemanticModelAsync(connectionId);
        if (semanticModel == null || semanticModel.BusinessGlossary == null)
        {
            return new Dictionary<string, GlossaryTerm>();
        }

        return semanticModel.BusinessGlossary;
    }

    public async Task<List<BusinessMetric>> GetBusinessMetricsAsync(string connectionId, string? businessArea = null)
    {
        var semanticModel = await GetSemanticModelAsync(connectionId);
        if (semanticModel == null || semanticModel.BusinessMetrics == null)
        {
            return new List<BusinessMetric>();
        }

        if (string.IsNullOrEmpty(businessArea))
        {
            return semanticModel.BusinessMetrics;
        }

        return semanticModel.BusinessMetrics.FindAll(m => 
            m.BusinessArea?.Equals(businessArea, StringComparison.OrdinalIgnoreCase) == true);
    }
}