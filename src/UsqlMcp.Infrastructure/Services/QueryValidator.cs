namespace UsqlMcp.Infrastructure.Services;

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models;

public class QueryValidator : IQueryValidator
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ISqlScriptProvider _sqlScriptProvider;
    private readonly ILogger<QueryValidator> _logger;

    public QueryValidator(
        IConnectionRepository connectionRepository,
        ISqlScriptProvider sqlScriptProvider,
        ILogger<QueryValidator> logger)
    {
        _connectionRepository = connectionRepository;
        _sqlScriptProvider = sqlScriptProvider;
        _logger = logger;
    }

    public async Task<bool> ValidateQueryAsync(string connectionId, string query)
    {
        var connection = await _connectionRepository.GetConnectionByIdAsync(connectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection with ID {ConnectionId} not found", connectionId);
            return false;
        }

        // If connection is read-only, we don't need to validate the query
        if (connection.ReadOnly)
        {
            return true;
        }

        // Normalize the query (remove comments, extra whitespace, make lowercase)
        var normalizedQuery = NormalizeQuery(query);

        // Check for write operations using regex patterns
        return !ContainsWriteOperations(normalizedQuery);
    }

    private string NormalizeQuery(string query)
    {
        // Remove SQL comments
        var withoutComments = Regex.Replace(query, "--.*$", "", RegexOptions.Multiline);
        withoutComments = Regex.Replace(withoutComments, "/\\*[\\s\\S]*?\\*/", "");

        // Normalize whitespace
        var normalized = Regex.Replace(withoutComments, "\\s+", " ").Trim().ToLowerInvariant();

        return normalized;
    }

    private bool ContainsWriteOperations(string normalizedQuery)
    {
        // Common write operation patterns
        var writePatterns = new[]
        {
            new Regex("\\s*insert\\s+"),
            new Regex("\\s*update\\s+"),
            new Regex("\\s*delete\\s+"),
            new Regex("\\s*truncate\\s+"),
            new Regex("\\s*drop\\s+"),
            new Regex("\\s*alter\\s+"),
            new Regex("\\s*create\\s+"),
            new Regex("\\s*grant\\s+"),
            new Regex("\\s*revoke\\s+"),
            new Regex("\\s*with\\s+.*\\s+update"),
            new Regex("\\s*with\\s+.*\\s+delete"),
            new Regex("\\s*with\\s+.*\\s+insert"),
            new Regex("\\s*set\\s+"),
            new Regex("\\s*exec\\s+"),
            new Regex("\\s*execute\\s+"),
            new Regex("\\s*call\\s+"),
            new Regex("\\s*begin\\s+"),
            new Regex("\\s*merge\\s+")
        };

        foreach (var pattern in writePatterns)
        {
            if (pattern.IsMatch(normalizedQuery))
            {
                return true;
            }
        }

        return false;
    }
}